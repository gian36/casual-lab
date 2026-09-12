// CL-01 engine spike — web + Capacitor slice. THROWAWAY.
//
// Deliberately the same slice as the Godot one, measured the same way: portrait 9:16,
// four lanes, drag a service from the tray, filling bar, completion pop plus one-shot,
// countdown to a fail state with retry, one persisted value, and a perf readout.
import Phaser from "phaser";

const W = 1080;
const H = 1920;

const SERVICE_NAMES = ["FUEL", "BAGS", "CATER", "PUSH"];
const SERVICE_COLOURS = [0xfabf40, 0x6bb8fa, 0xb88cf2, 0x73e09e];
const LANE_COUNT = 4;
const SERVICE_SECONDS = 3.0;
const SHIFT_SECONDS = 45.0;
const BEST_KEY = "cl01.best";

// FLAGGED TO GIAN 2026-09-12, UNRULED — same flag as the Godot slice. -1 keeps the
// briefed behaviour (unlimited vehicles). Set to 2 to feel contention on four lanes.
const VEHICLE_POOL = -1;

const LANE_TOP = 230;
const LANE_H = 300;
const LANE_GAP = 16;
const TRAY_TOP = 1580;
const TRAY_H = 300;
const ICON = 64;
const ICON_GAP = 14;

// Same nose-left silhouette as the Godot slice, in the same unit space, so the two
// are drawing comparable geometry rather than comparable-looking geometry.
const PLANE: [number, number][] = [
  [0.0, 0.5], [0.09, 0.36], [0.34, 0.33], [0.4, 0.06], [0.48, 0.06], [0.49, 0.33],
  [0.78, 0.33], [0.9, 0.1], [0.97, 0.1], [0.94, 0.36], [1.0, 0.44], [1.0, 0.56],
  [0.94, 0.64], [0.97, 0.9], [0.9, 0.9], [0.78, 0.67], [0.49, 0.67], [0.48, 0.94],
  [0.4, 0.94], [0.34, 0.67], [0.09, 0.64],
];

interface Pop {
  service: number;
  age: number;
}

class Lane {
  needs: number[] = [];
  activeService = -1;
  activeElapsed = 0;
  pops: Pop[] = [];
  refuseFlash = 0;
  hoverOk = false;
  hoverBad = false;

  constructor(
    readonly index: number,
    readonly x: number,
    readonly y: number,
    readonly w: number,
    readonly h: number,
  ) {}

  get rect() {
    return new Phaser.Geom.Rectangle(this.x, this.y, this.w, this.h);
  }

  isClean() {
    return this.needs.length === 0 && this.activeService === -1;
  }

  reset(needs: number[]) {
    this.needs = needs.slice();
    this.activeService = -1;
    this.activeElapsed = 0;
    this.pops = [];
    this.refuseFlash = 0;
  }

  accepts(service: number) {
    return this.activeService === -1 && this.needs.includes(service);
  }

  // Returns the service index that just finished, or -1.
  tick(dt: number): number {
    let completed = -1;
    if (this.activeService !== -1) {
      this.activeElapsed += dt;
      if (this.activeElapsed >= SERVICE_SECONDS) {
        completed = this.activeService;
        this.needs = this.needs.filter((n) => n !== completed);
        this.pops.push({ service: completed, age: 0 });
        this.activeService = -1;
        this.activeElapsed = 0;
      }
    }
    for (const pop of this.pops) pop.age += dt;
    this.pops = this.pops.filter((p) => p.age < 0.55);
    if (this.refuseFlash > 0) this.refuseFlash = Math.max(0, this.refuseFlash - dt * 3);
    return completed;
  }
}

// Procedurally generated one-shots, so the slice ships no audio asset — the same
// choice as the Godot slice, and it exercises the real audio path in the WebView.
class Sfx {
  private ctx: AudioContext | null = null;
  private buffers = new Map<string, AudioBuffer>();

  // A WebView will not start audio before a user gesture. The native candidates do
  // not have this problem; it is recorded as a finding, not worked around silently.
  unlock() {
    if (!this.ctx) {
      const Ctor = window.AudioContext ?? (window as any).webkitAudioContext;
      this.ctx = new Ctor();
      this.build("pop", 0.14, (t) => {
        const env = Math.exp(-t * 26);
        const freq = 760 + 620 * Math.exp(-t * 16);
        return Math.sin(2 * Math.PI * freq * t) * env * 0.55;
      });
      this.build("reject", 0.1, (t) => Math.sin(2 * Math.PI * 190 * t) * Math.exp(-t * 30) * 0.35);
    }
    if (this.ctx.state === "suspended") void this.ctx.resume();
  }

  private build(name: string, duration: number, shape: (t: number) => number) {
    const ctx = this.ctx!;
    const frames = Math.floor(ctx.sampleRate * duration);
    const buffer = ctx.createBuffer(1, frames, ctx.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < frames; i++) data[i] = shape(i / ctx.sampleRate);
    this.buffers.set(name, buffer);
  }

  play(name: string) {
    if (!this.ctx) return;
    const buffer = this.buffers.get(name);
    if (!buffer) return;
    const source = this.ctx.createBufferSource();
    source.buffer = buffer;
    source.connect(this.ctx.destination);
    source.start();
  }
}

class SpikeScene extends Phaser.Scene {
  private lanes: Lane[] = [];
  private available: number[] = [];
  private g!: Phaser.GameObjects.Graphics;
  private stressG!: Phaser.GameObjects.Graphics;
  private hud!: Phaser.GameObjects.Text;
  private perf!: Phaser.GameObjects.Text;
  private failText!: Phaser.GameObjects.Text;
  private sfx = new Sfx();

  private timeLeft = SHIFT_SECONDS;
  private served = 0;
  private best = 0;
  private running = true;

  private accumulated = 0;
  private startedAt = 0;
  private frameMax = 0;
  private dragFrameMax = 0;
  private perfWindow = 0;
  private dragging: { service: number; x: number; y: number } | null = null;
  private stress = 0;
  private needLabels: Phaser.GameObjects.Text[][] = [];

  constructor() {
    super("spike");
  }

  create() {
    this.best = Number(localStorage.getItem(BEST_KEY) ?? 0);

    for (let i = 0; i < LANE_COUNT; i++) {
      this.lanes.push(new Lane(i, 24, LANE_TOP + i * (LANE_H + LANE_GAP), W - 48, LANE_H));
    }

    this.stressG = this.add.graphics();
    this.g = this.add.graphics();

    this.hud = this.add.text(32, 18, "", { fontFamily: "sans-serif", fontSize: "64px", color: "#ffffff" });
    this.perf = this.add
      .text(W - 32, 24, "", { fontFamily: "monospace", fontSize: "28px", color: "#80bf8c", align: "right" })
      .setOrigin(1, 0);
    this.failText = this.add
      .text(W / 2, H / 2, "", { fontFamily: "sans-serif", fontSize: "88px", color: "#ff6b61", align: "center" })
      .setOrigin(0.5)
      .setVisible(false);

    this.buildLabels();
    this.buildTray();
    this.buildButtons();

    this.input.on("pointerdown", () => this.sfx.unlock());
    this.restart();

    // Spike-only handle, so the slice can be inspected from a devtools console or
    // from `adb`-driven remote debugging once it is on the phone.
    (window as unknown as { __spike: SpikeScene }).__spike = this;
  }

  // Parity with the Godot slice: the need icons and the tray are labelled. Text
  // objects are created once and repositioned, never created per frame.
  private buildLabels() {
    for (const lane of this.lanes) {
      const row: Phaser.GameObjects.Text[] = [];
      for (let i = 0; i < SERVICE_NAMES.length; i++) {
        row.push(
          this.add
            .text(0, 0, "", { fontFamily: "sans-serif", fontSize: "20px", color: "#bfc7d1" })
            .setVisible(false),
        );
      }
      this.needLabels.push(row);
    }
  }

  private buildTray() {
    const slotW = (W - 48 - 3 * 16) / 4;
    for (let s = 0; s < SERVICE_NAMES.length; s++) {
      const x = 24 + s * (slotW + 16);
      this.add.text(x + 18, TRAY_TOP + TRAY_H / 2 - 22, SERVICE_NAMES[s], {
        fontFamily: "sans-serif",
        fontSize: "34px",
        color: "#141a22",
      });
      const zone = this.add
        .zone(x, TRAY_TOP, slotW, TRAY_H)
        .setOrigin(0)
        .setInteractive({ draggable: true });
      zone.on("dragstart", (p: Phaser.Input.Pointer) => {
        if (this.available[s] === 0) return;
        this.dragging = { service: s, x: p.worldX, y: p.worldY };
      });
      zone.on("drag", (p: Phaser.Input.Pointer) => {
        if (this.dragging) {
          this.dragging.x = p.worldX;
          this.dragging.y = p.worldY;
        }
      });
      zone.on("dragend", (p: Phaser.Input.Pointer) => {
        if (!this.dragging) return;
        const service = this.dragging.service;
        this.dragging = null;
        const lane = this.lanes.find((l) => Phaser.Geom.Rectangle.Contains(l.rect, p.worldX, p.worldY));
        if (!lane) return;
        if (lane.accepts(service) && this.take(service)) {
          lane.activeService = service;
          lane.activeElapsed = 0;
        } else {
          lane.refuseFlash = 1;
          this.sfx.play("reject");
        }
      });
    }
  }

  private buildButtons() {
    const retry = this.add
      .text(W / 2, H / 2 + 220, "RETRY", {
        fontFamily: "sans-serif",
        fontSize: "56px",
        color: "#ffffff",
        backgroundColor: "#3a4152",
        padding: { x: 60, y: 26 },
      })
      .setOrigin(0.5)
      .setInteractive({ useHandCursor: true })
      .setVisible(false);
    retry.on("pointerdown", () => this.restart());
    this.data.set("retry", retry);

    const stress = this.add
      .text(W - 24, TRAY_TOP - 84, "stress +100", {
        fontFamily: "monospace",
        fontSize: "28px",
        color: "#ffffff",
        backgroundColor: "#2a3040",
        padding: { x: 24, y: 14 },
      })
      .setOrigin(1, 0)
      .setInteractive({ useHandCursor: true });
    stress.on("pointerdown", () => {
      this.stress += 100;
      this.drawStress();
    });
  }

  private take(service: number): boolean {
    if (VEHICLE_POOL < 0) return true;
    if (this.available[service] <= 0) return false;
    this.available[service] -= 1;
    return true;
  }

  private randomNeeds(): number[] {
    const pool = [0, 1, 2, 3];
    Phaser.Utils.Array.Shuffle(pool);
    return pool.slice(0, Phaser.Math.Between(2, 3));
  }

  private restart() {
    this.timeLeft = SHIFT_SECONDS;
    this.served = 0;
    this.running = true;
    this.accumulated = 0;
    this.startedAt = performance.now();
    this.available = SERVICE_NAMES.map(() => VEHICLE_POOL);
    for (const lane of this.lanes) lane.reset(this.randomNeeds());
    this.failText.setVisible(false);
    (this.data.get("retry") as Phaser.GameObjects.Text).setVisible(false);
  }

  update(_time: number, deltaMs: number) {
    const dt = deltaMs / 1000;
    this.measure(dt);

    if (this.running) {
      this.accumulated += dt;
      this.timeLeft = Math.max(0, SHIFT_SECONDS - this.accumulated);

      for (const lane of this.lanes) {
        const completed = lane.tick(dt);
        if (completed !== -1) {
          this.served += 1;
          if (VEHICLE_POOL >= 0) {
            this.available[completed] = Math.min(this.available[completed] + 1, VEHICLE_POOL);
          }
          this.sfx.play("pop");
          if (lane.isClean()) lane.reset(this.randomNeeds());
        }
      }
      if (this.timeLeft <= 0) this.fail();
    }

    this.draw();
  }

  private fail() {
    this.running = false;
    if (this.served > this.best) {
      this.best = this.served;
      try {
        localStorage.setItem(BEST_KEY, String(this.best));
      } catch (err) {
        console.warn("could not persist best score", err);
      }
    }
    this.failText.setText(`SHIFT FAILED\nserved ${this.served} · best ${this.best}`).setVisible(true);
    (this.data.get("retry") as Phaser.GameObjects.Text).setVisible(true);
  }

  private measure(dt: number) {
    const frameMs = dt * 1000;
    this.frameMax = Math.max(this.frameMax, frameMs);
    if (this.dragging) this.dragFrameMax = Math.max(this.dragFrameMax, frameMs);
    this.perfWindow += dt;
    if (this.perfWindow >= 1) {
      const wall = (performance.now() - this.startedAt) / 1000;
      const drift = this.accumulated - wall;
      this.perf.setText(
        [
          `fps ${Math.round(this.game.loop.actualFps)}`,
          `frame max ${this.frameMax.toFixed(1)} ms`,
          `drag max ${this.dragFrameMax.toFixed(1)} ms`,
          `drift ${drift >= 0 ? "+" : ""}${drift.toFixed(3)} s`,
          `planes ${LANE_COUNT + this.stress}`,
        ].join("\n"),
      );
      this.frameMax = 0;
      this.dragFrameMax = 0;
      this.perfWindow = 0;
    }
  }

  // Phaser 4 dropped Geom.Point, and its fillPoints signature wants real Vector2s
  // rather than plain {x, y} literals. Two separate API changes, both caught by tsc.
  private planePoints(x: number, y: number, w: number, h: number) {
    return PLANE.map(([ux, uy]) => new Phaser.Math.Vector2(x + ux * w, y + uy * h));
  }

  private draw() {
    const g = this.g;
    g.clear();

    for (const lane of this.lanes) {
      let bg = 0x212630;
      if (lane.hoverOk) bg = 0x293f31;
      else if (lane.refuseFlash > 0) bg = 0x422630;
      g.fillStyle(bg, 1).fillRoundedRect(lane.x, lane.y, lane.w, lane.h, 18);

      g.fillStyle(lane.isClean() ? 0x59d973 : 0xd9a640, 1).fillCircle(lane.x + 30, lane.y + lane.h / 2, 12);

      const body = lane.isClean() ? 0x8ce6a6 : 0xd1d9e6;
      g.fillStyle(body, 1).fillPoints(
        this.planePoints(lane.x + 58, lane.y + lane.h * 0.22, lane.w * 0.42, lane.h * 0.56),
        true,
      );

      let ix = lane.x + lane.w * 0.52;
      const iy = lane.y + lane.h / 2 - ICON / 2;
      const labels = this.needLabels[lane.index];
      for (const label of labels) label.setVisible(false);
      lane.needs.forEach((service, slot) => {
        const label = labels[slot];
        label.setPosition(ix - 2, iy + ICON + 6).setText(SERVICE_NAMES[service]).setVisible(true);
        const colour = SERVICE_COLOURS[service];
        if (service === lane.activeService) {
          g.fillStyle(0x3a3a3a, 1).fillRoundedRect(ix, iy, ICON, ICON, 10);
          const frac = Phaser.Math.Clamp(lane.activeElapsed / SERVICE_SECONDS, 0, 1);
          g.fillStyle(colour, 1).fillRoundedRect(ix, iy + ICON * (1 - frac), ICON, ICON * frac, 10);
        } else {
          g.fillStyle(colour, 1).fillRoundedRect(ix, iy, ICON, ICON, 10);
        }
        ix += ICON + ICON_GAP;
      });

      for (const pop of lane.pops) {
        const t = Phaser.Math.Clamp(pop.age / 0.55, 0, 1);
        const cx = lane.x + lane.w * 0.52 + ICON / 2;
        const cy = lane.y + lane.h / 2;
        g.lineStyle(5, SERVICE_COLOURS[pop.service], 1 - t).strokeCircle(cx, cy, ICON * 0.6 * (1 + 1.6 * t));
        const s = ICON * (1 + 0.5 * (1 - t));
        g.fillStyle(SERVICE_COLOURS[pop.service], 1 - t).fillRoundedRect(cx - s / 2, cy - s / 2, s, s, 10);
      }
    }

    const slotW = (W - 48 - 3 * 16) / 4;
    for (let s = 0; s < SERVICE_NAMES.length; s++) {
      const x = 24 + s * (slotW + 16);
      const dim = this.available[s] === 0;
      g.fillStyle(SERVICE_COLOURS[s], dim ? 0.35 : 1).fillRoundedRect(x, TRAY_TOP, slotW, TRAY_H, 18);
    }

    if (this.dragging) {
      g.fillStyle(SERVICE_COLOURS[this.dragging.service], 0.9).fillRoundedRect(
        this.dragging.x - 60,
        this.dragging.y - 60,
        120,
        120,
        16,
      );
    }

    this.hud.setText(`${this.timeLeft.toFixed(1)}\nSERVED ${this.served}   BEST ${this.best}`);
  }

  private drawStress() {
    this.stressG.clear();
    if (this.stress <= 0) return;
    const rng = new Phaser.Math.RandomDataGenerator(["cl01"]);
    this.stressG.fillStyle(0xd1d9e6, 0.18);
    for (let i = 0; i < this.stress; i++) {
      const x = rng.between(0, W - 160);
      const y = rng.between(LANE_TOP, 1500);
      this.stressG.fillPoints(this.planePoints(x, y, 150, 60), true);
    }
  }
}

// Tray labels are DOM-free: drawn by Phaser text objects created once, over the canvas.
new Phaser.Game({
  type: Phaser.AUTO,
  parent: "game",
  width: W,
  height: H,
  backgroundColor: "#131620",
  scale: { mode: Phaser.Scale.FIT, autoCenter: Phaser.Scale.CENTER_BOTH },
  scene: [SpikeScene],
});

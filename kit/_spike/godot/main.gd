# CL-01 engine spike — the whole slice, per HANDOFF_KIT_ENGINE_SPIKE.md. THROWAWAY.
#
# Four lanes, an aircraft each, a tray you drag services from, a filling bar, a pop
# and a one-shot on completion, a countdown that fails the shift, a retry, and one
# persisted value. Nothing here is the kit and nothing here is 005.
extends Control

const SERVICE_NAMES: Array[String] = ["FUEL", "BAGS", "CATER", "PUSH"]
const LANE_COUNT := 4
const SERVICE_SECONDS := 3.0
const SHIFT_SECONDS := 45.0
const SAVE_PATH := "user://best.cfg"

# FLAGGED TO GIAN 2026-09-12, UNRULED. The brief's slice has no scarcity, but 005's
# tactical layer *is* allocation under contention (PORTFOLIO C02 §4). -1 keeps the
# briefed behaviour: unlimited vehicles. Set to 2 to feel contention on four lanes.
const VEHICLE_POOL := -1

var service_colours: Array[Color] = [
	Color(0.98, 0.75, 0.25),  # FUEL
	Color(0.42, 0.72, 0.98),  # BAGS
	Color(0.72, 0.55, 0.95),  # CATER
	Color(0.45, 0.88, 0.62),  # PUSH
]

var _lanes: Array[LanePanel] = []
var _tray: Array[TrayButton] = []
var _in_use: Array[int] = []  # lane index per busy vehicle, for the scarcity variant

var _time_left := SHIFT_SECONDS
var _served := 0
var _best := 0
var _running := true

var _pop_sound: AudioStreamWAV
var _reject_sound: AudioStreamWAV

# Measurement, for the report's "iteration / build reality" and 005 feasibility rows.
var _shift_started_ms := 0
var _accumulated := 0.0
var _frame_max_ms := 0.0
var _drag_frame_max_ms := 0.0
var _perf_window := 0.0
var _fps := 0.0
var _stress := 0
var _stress_layer: Control


func _ready() -> void:
	randomize()
	_pop_sound = SpikeSfx.complete_pop()
	_reject_sound = SpikeSfx.reject()
	_best = _load_best()

	_stress_layer = Control.new()
	_stress_layer.name = "StressLayer"
	_stress_layer.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_stress_layer.set_anchors_preset(Control.PRESET_FULL_RECT)
	_stress_layer.draw.connect(_draw_stress)
	add_child(_stress_layer)
	move_child(_stress_layer, 1)

	_build_lanes()
	_build_tray()
	if "--shot" in OS.get_cmdline_user_args():
		_capture_after_frames()
	%RetryButton.pressed.connect(_restart)
	%StressButton.pressed.connect(func() -> void:
		_stress += 100
		_stress_layer.queue_redraw()
	)
	_restart()


func _build_lanes() -> void:
	var container: VBoxContainer = $Lanes
	for i in LANE_COUNT:
		var lane := LanePanel.new()
		lane.name = "Lane%d" % i
		lane.lane_index = i
		lane.service_names = SERVICE_NAMES
		lane.service_colors = service_colours
		lane.service_requested.connect(_on_service_requested)
		container.add_child(lane)
		_lanes.push_back(lane)


func _build_tray() -> void:
	var container: HBoxContainer = $Tray
	for s in SERVICE_NAMES.size():
		var button := TrayButton.new()
		button.name = "Tray%d" % s
		button.service = s
		button.label = SERVICE_NAMES[s]
		button.colour = service_colours[s]
		button.available = VEHICLE_POOL
		button.drag_failed.connect(_on_drag_failed)
		container.add_child(button)
		_tray.push_back(button)


func _restart() -> void:
	_time_left = SHIFT_SECONDS
	_served = 0
	_running = true
	_in_use.clear()
	_accumulated = 0.0
	_shift_started_ms = Time.get_ticks_msec()
	for lane in _lanes:
		lane.reset(_random_needs())
	for button in _tray:
		button.available = VEHICLE_POOL
		button.queue_redraw()
	%FailOverlay.visible = false
	_refresh_hud()


func _random_needs() -> Array[int]:
	var pool: Array[int] = [0, 1, 2, 3]
	pool.shuffle()
	var wanted := randi_range(2, 3)
	var picked: Array[int] = []
	for i in wanted:
		picked.push_back(pool[i])
	return picked


# --- the loop ----------------------------------------------------------------------

func _process(delta: float) -> void:
	_measure(delta)
	if not _running:
		return

	_accumulated += delta
	_time_left = maxf(0.0, SHIFT_SECONDS - _accumulated)

	for lane in _lanes:
		var completed := lane.tick(delta)
		if completed != -1:
			_served += 1
			_free_vehicle(completed, lane.lane_index)
			%Sfx.stream = _pop_sound
			%Sfx.play()
			if lane.is_clean():
				# Aircraft pushes back; a new one takes the gate. Keeps the shift moving.
				lane.reset(_random_needs())

	if _time_left <= 0.0:
		_fail()
	_refresh_hud()


func _on_service_requested(lane: LanePanel, service: int) -> void:
	if not _running or lane.active_service != -1:
		return
	if VEHICLE_POOL >= 0:
		var button := _tray[service]
		if button.available <= 0:
			lane.refuse(service)
			_play_reject()
			return
		button.available -= 1
		button.queue_redraw()
	lane.start_service(service, SERVICE_SECONDS)


func _free_vehicle(service: int, _lane_index: int) -> void:
	if VEHICLE_POOL < 0:
		return
	var button := _tray[service]
	button.available = mini(button.available + 1, VEHICLE_POOL)
	button.queue_redraw()


# A drag that no lane accepted. Flash whichever lane the finger was over, so the
# refusal is legible rather than silent.
func _on_drag_failed(service: int) -> void:
	var mouse := get_global_mouse_position()
	for lane in _lanes:
		if lane.get_global_rect().has_point(mouse):
			lane.refuse(service)
			_play_reject()
			return


func _play_reject() -> void:
	%Sfx.stream = _reject_sound
	%Sfx.play()


func _fail() -> void:
	_running = false
	if _served > _best:
		_best = _served
		_save_best(_best)
	%FailDetail.text = "served %d   ·   best %d" % [_served, _best]
	%FailOverlay.visible = true


# --- hud, persistence, measurement -------------------------------------------------

func _refresh_hud() -> void:
	%TimeLabel.text = "%0.1f" % _time_left
	%ScoreLabel.text = "SERVED %d" % _served
	%BestLabel.text = "BEST %d" % _best


func _load_best() -> int:
	var cfg := ConfigFile.new()
	if cfg.load(SAVE_PATH) != OK:
		return 0
	return int(cfg.get_value("score", "best", 0))


func _save_best(value: int) -> void:
	var cfg := ConfigFile.new()
	cfg.set_value("score", "best", value)
	var err := cfg.save(SAVE_PATH)
	if err != OK:
		push_warning("could not persist best score: %d" % err)


func _measure(delta: float) -> void:
	var frame_ms := delta * 1000.0
	_frame_max_ms = maxf(_frame_max_ms, frame_ms)
	if get_viewport().gui_is_dragging():
		_drag_frame_max_ms = maxf(_drag_frame_max_ms, frame_ms)
	_perf_window += delta
	if _perf_window >= 1.0:
		_fps = Engine.get_frames_per_second()
		# Timer accuracy: accumulated delta against the OS clock since the shift began.
		var wall := float(Time.get_ticks_msec() - _shift_started_ms) / 1000.0
		var drift := _accumulated - wall
		%PerfLabel.text = "fps %0.0f\nframe max %0.1f ms\ndrag max %0.1f ms\ndrift %+0.3f s\nplanes %d" % [
			_fps, _frame_max_ms, _drag_frame_max_ms, drift, LANE_COUNT + _stress,
		]
		_frame_max_ms = 0.0
		_drag_frame_max_ms = 0.0
		_perf_window = 0.0


# Run with `-- --shot` to save the game's own viewport to user://shot.png and quit.
# Deliberately an engine-side capture, never a desktop screen grab: it can only ever
# see this game's viewport.
func _capture_after_frames(frames: int = 90) -> void:
	for i in frames:
		await get_tree().process_frame
	var image := get_viewport().get_texture().get_image()
	image.save_png("user://shot.png")
	print("shot saved: ", ProjectSettings.globalize_path("user://shot.png"))
	get_tree().quit()


# Stress layer: extra aircraft silhouettes, to find where frame time degrades.
# Answers the brief's "sprite counts" observation for 005.
func _draw_stress() -> void:
	if _stress <= 0:
		return
	var rng := RandomNumberGenerator.new()
	rng.seed = 12345
	var faint := Color(0.82, 0.85, 0.9, 0.18)
	for i in _stress:
		var pos := Vector2(rng.randf_range(0.0, size.x - 160.0), rng.randf_range(220.0, 1500.0))
		var plane := Rect2(pos, Vector2(150.0, 60.0))
		_stress_layer.draw_colored_polygon(SpikeArt.aircraft_points(plane), faint)

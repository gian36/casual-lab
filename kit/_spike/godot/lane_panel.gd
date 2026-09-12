# One gate lane: an aircraft, its outstanding need icons, and the bar for the service
# currently running. Also the drop target for the tray. THROWAWAY.
class_name LanePanel
extends Control

signal service_requested(lane: LanePanel, service: int)
signal drop_refused(lane: LanePanel, service: int)

const ICON := 64.0
const ICON_GAP := 14.0

var lane_index := 0
var service_names: Array[String] = []
var service_colors: Array[Color] = []

var needs: Array[int] = []
var active_service := -1
var active_elapsed := 0.0
var active_total := 3.0
var served := 0
var pops: Array = []  # [{service:int, age:float}]

var _hover_ok_frame := -10
var _hover_bad_frame := -10
var _refuse_flash := 0.0


func _ready() -> void:
	custom_minimum_size = Vector2(0, 300)
	size_flags_vertical = Control.SIZE_EXPAND_FILL
	mouse_filter = Control.MOUSE_FILTER_STOP


func reset(new_needs: Array[int]) -> void:
	needs = new_needs.duplicate()
	active_service = -1
	active_elapsed = 0.0
	served = 0
	pops.clear()
	_refuse_flash = 0.0
	queue_redraw()


func is_clean() -> bool:
	return needs.is_empty() and active_service == -1


# --- drag and drop -----------------------------------------------------------------
# Godot calls _can_drop_data every frame while a drag hovers this control, so it
# doubles as the hover-feedback hook. Frame stamps are used instead of a boolean
# because there is no "drag left me" callback to clear it.

func _can_drop_data(_at_position: Vector2, data: Variant) -> bool:
	var service := _service_of(data)
	if service == -1:
		return false
	var ok := active_service == -1 and needs.has(service)
	if ok:
		_hover_ok_frame = Engine.get_process_frames()
	else:
		_hover_bad_frame = Engine.get_process_frames()
	return ok


func _drop_data(_at_position: Vector2, data: Variant) -> void:
	var service := _service_of(data)
	if service != -1:
		service_requested.emit(self, service)


# A refused drag never reaches _drop_data, so the tray reports it here instead.
func refuse(service: int) -> void:
	_refuse_flash = 1.0
	drop_refused.emit(self, service)
	queue_redraw()


func _service_of(data: Variant) -> int:
	if typeof(data) != TYPE_DICTIONARY:
		return -1
	var d: Dictionary = data
	if not d.has("service"):
		return -1
	return int(d["service"])


# --- tick and draw -----------------------------------------------------------------

func tick(delta: float) -> int:
	var completed := -1
	if active_service != -1:
		active_elapsed += delta
		if active_elapsed >= active_total:
			completed = active_service
			needs.erase(active_service)
			pops.push_back({"service": active_service, "age": 0.0})
			served += 1
			active_service = -1
			active_elapsed = 0.0
	for pop in pops:
		pop["age"] += delta
	pops = pops.filter(func(p: Dictionary) -> bool: return p["age"] < 0.55)
	if _refuse_flash > 0.0:
		_refuse_flash = maxf(0.0, _refuse_flash - delta * 3.0)
	queue_redraw()
	return completed


func start_service(service: int, seconds: float) -> void:
	active_service = service
	active_total = seconds
	active_elapsed = 0.0
	queue_redraw()


func _draw() -> void:
	var font := ThemeDB.fallback_font
	var r := Rect2(Vector2.ZERO, size)
	var frame := Engine.get_process_frames()
	var hovering_ok := frame - _hover_ok_frame <= 1
	var hovering_bad := frame - _hover_bad_frame <= 1

	var bg := Color(0.13, 0.15, 0.19)
	if hovering_ok:
		bg = Color(0.16, 0.25, 0.19)
	elif hovering_bad or _refuse_flash > 0.0:
		bg = Color(0.26, 0.15, 0.16).lerp(Color(0.13, 0.15, 0.19), 1.0 - maxf(_refuse_flash, 0.6 if hovering_bad else 0.0))
	SpikeArt.rounded(self, r, bg, 18.0)

	# Gate light: green once the aircraft is clean.
	var light := Color(0.35, 0.85, 0.45) if is_clean() else Color(0.85, 0.65, 0.25)
	draw_circle(Vector2(30.0, size.y * 0.5), 12.0, light)

	# Aircraft, nose-left.
	var plane_rect := Rect2(Vector2(58.0, size.y * 0.22), Vector2(size.x * 0.42, size.y * 0.56))
	var body := Color(0.82, 0.85, 0.9) if not is_clean() else Color(0.55, 0.9, 0.65)
	draw_colored_polygon(SpikeArt.aircraft_points(plane_rect), body)

	# Outstanding needs, as icons to the right of the aircraft.
	var x := size.x * 0.52
	var y := size.y * 0.5 - ICON * 0.5
	for service in needs:
		var col: Color = service_colors[service]
		var icon_rect := Rect2(Vector2(x, y), Vector2(ICON, ICON))
		if service == active_service:
			# Active service: the icon becomes a filling bar (C02 §3 feedback).
			SpikeArt.rounded(self, icon_rect, col.darkened(0.65), 10.0)
			var frac: float = clampf(active_elapsed / active_total, 0.0, 1.0)
			var fill := Rect2(icon_rect.position + Vector2(0, ICON * (1.0 - frac)), Vector2(ICON, ICON * frac))
			SpikeArt.rounded(self, fill, col, 10.0)
		else:
			SpikeArt.rounded(self, icon_rect, col, 10.0)
		draw_string(font, Vector2(x - 2.0, y + ICON + 26.0), service_names[service],
				HORIZONTAL_ALIGNMENT_LEFT, -1, 20, Color(0.75, 0.78, 0.82))
		x += ICON + ICON_GAP

	# Completion pop: the icon scales up and fades, with a ring behind it.
	for pop in pops:
		var age: float = pop["age"]
		var t: float = clampf(age / 0.55, 0.0, 1.0)
		var col: Color = service_colors[int(pop["service"])]
		var centre := Vector2(size.x * 0.52 + ICON * 0.5, size.y * 0.5)
		var scale: float = 1.0 + 1.6 * t
		var alpha: float = 1.0 - t
		draw_arc(centre, ICON * 0.6 * scale, 0.0, TAU, 28, Color(col.r, col.g, col.b, alpha * 0.8), 5.0)
		var s := ICON * (1.0 + 0.5 * (1.0 - t))
		SpikeArt.rounded(self, Rect2(centre - Vector2(s, s) * 0.5, Vector2(s, s)),
				Color(col.r, col.g, col.b, alpha), 10.0)

	draw_string(font, Vector2(size.x - 96.0, 38.0), "G%d" % (lane_index + 1),
			HORIZONTAL_ALIGNMENT_LEFT, -1, 28, Color(0.45, 0.48, 0.54))

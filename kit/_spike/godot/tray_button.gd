# One service vehicle in the bottom tray. Drag source. THROWAWAY.
class_name TrayButton
extends Control

signal drag_failed(service: int)

var service := 0
var label := ""
var colour := Color.WHITE
var available := -1  # -1 == unlimited

var _dragging := false


func _ready() -> void:
	size_flags_horizontal = Control.SIZE_EXPAND_FILL
	mouse_filter = Control.MOUSE_FILTER_STOP


func _get_drag_data(_at_position: Vector2) -> Variant:
	if available == 0:
		return null
	_dragging = true
	var preview := Control.new()
	preview.custom_minimum_size = Vector2(120, 120)
	var chip := ColorRect.new()
	chip.color = colour
	chip.size = Vector2(120, 120)
	chip.position = Vector2(-60, -60)  # centre the preview on the finger
	preview.add_child(chip)
	set_drag_preview(preview)
	return {"service": service}


# A refused drag never reaches any _drop_data, so failure is detected here instead:
# the drag ends and the viewport reports it as unsuccessful.
func _notification(what: int) -> void:
	if what == NOTIFICATION_DRAG_END and _dragging:
		_dragging = false
		if not get_viewport().gui_is_drag_successful():
			drag_failed.emit(service)


func _draw() -> void:
	var font := ThemeDB.fallback_font
	var r := Rect2(Vector2.ZERO, size)
	var body := colour if available != 0 else colour.darkened(0.65)
	SpikeArt.rounded(self, r, body, 18.0)
	draw_string(font, Vector2(18.0, size.y * 0.52), label,
			HORIZONTAL_ALIGNMENT_LEFT, -1, 34, Color(0.08, 0.09, 0.11))
	if available >= 0:
		draw_string(font, Vector2(18.0, size.y * 0.52 + 44.0), "x%d" % available,
				HORIZONTAL_ALIGNMENT_LEFT, -1, 30, Color(0.08, 0.09, 0.11, 0.75))

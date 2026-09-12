# Drawing helpers for the CL-01 slice. No image assets on purpose: the slice is
# throwaway and the whole project stays text-reviewable. THROWAWAY.
class_name SpikeArt
extends RefCounted

# A nose-left aircraft silhouette, as PORTFOLIO C02 §1 describes the scene reading
# horizontally inside a tall screen. Normalised to the given rect.
static func aircraft_points(rect: Rect2) -> PackedVector2Array:
	var w := rect.size.x
	var h := rect.size.y
	var p := rect.position
	# Unit-space outline (0..1, nose at x=0), then scaled into rect.
	var unit := [
		Vector2(0.00, 0.50), Vector2(0.09, 0.36), Vector2(0.34, 0.33),
		Vector2(0.40, 0.06), Vector2(0.48, 0.06), Vector2(0.49, 0.33),
		Vector2(0.78, 0.33), Vector2(0.90, 0.10), Vector2(0.97, 0.10),
		Vector2(0.94, 0.36), Vector2(1.00, 0.44), Vector2(1.00, 0.56),
		Vector2(0.94, 0.64), Vector2(0.97, 0.90), Vector2(0.90, 0.90),
		Vector2(0.78, 0.67), Vector2(0.49, 0.67), Vector2(0.48, 0.94),
		Vector2(0.40, 0.94), Vector2(0.34, 0.67), Vector2(0.09, 0.64),
	]
	var out := PackedVector2Array()
	for u in unit:
		out.push_back(p + Vector2(u.x * w, u.y * h))
	return out


static func rounded(ci: CanvasItem, rect: Rect2, color: Color, radius: float = 12.0) -> void:
	# Cheap rounded rect: one body rect plus four corner circles. Good enough for a spike.
	ci.draw_rect(Rect2(rect.position + Vector2(radius, 0), rect.size - Vector2(radius * 2.0, 0)), color)
	ci.draw_rect(Rect2(rect.position + Vector2(0, radius), Vector2(rect.size.x, rect.size.y - radius * 2.0)), color)
	ci.draw_circle(rect.position + Vector2(radius, radius), radius, color)
	ci.draw_circle(rect.position + Vector2(rect.size.x - radius, radius), radius, color)
	ci.draw_circle(rect.position + Vector2(radius, rect.size.y - radius), radius, color)
	ci.draw_circle(rect.position + Vector2(rect.size.x - radius, rect.size.y - radius), radius, color)

# Procedurally generated one-shot, so the slice ships no binary audio asset while
# still exercising Godot's real audio path on device. THROWAWAY.
class_name SpikeSfx
extends RefCounted

const MIX_RATE := 22050


# Short bright blip for "service complete". Exponentially decaying pitch and volume.
static func complete_pop() -> AudioStreamWAV:
	return _tone(0.14, func(t: float) -> float:
		var env: float = exp(-t * 26.0)
		var freq: float = 760.0 + 620.0 * exp(-t * 16.0)
		return sin(TAU * freq * t) * env * 0.55
	)


# Duller, lower buzz for "that drop was refused".
static func reject() -> AudioStreamWAV:
	return _tone(0.10, func(t: float) -> float:
		var env: float = exp(-t * 30.0)
		return sin(TAU * 190.0 * t) * env * 0.35
	)


static func _tone(duration: float, shape: Callable) -> AudioStreamWAV:
	var frames := int(MIX_RATE * duration)
	var data := PackedByteArray()
	data.resize(frames * 2)  # 16-bit mono
	for i in frames:
		var t := float(i) / float(MIX_RATE)
		var s: float = clampf(shape.call(t), -1.0, 1.0)
		data.encode_s16(i * 2, int(s * 32767.0))
	var stream := AudioStreamWAV.new()
	stream.format = AudioStreamWAV.FORMAT_16_BITS
	stream.mix_rate = MIX_RATE
	stream.stereo = false
	stream.data = data
	return stream

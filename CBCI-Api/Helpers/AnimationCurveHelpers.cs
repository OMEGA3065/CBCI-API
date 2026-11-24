using NorthwoodLib.Pools;
using UnityEngine;
using Utils;

namespace CustomItemLib.Helpers;

public static class AnimationCurveHelpers
{
    public static AnimationColorCurve GetRainbowCurve(float duration)
    {
        var frameDuration = duration * (1 / 5f);
        var slopeValue = 1f / frameDuration; // 1f is value delta.

        return new AnimationColorCurve(
            new AnimationCurve(
                new Keyframe(0, 1f, 0, 0),
                new Keyframe(frameDuration, 1f, 0, -slopeValue),
                new Keyframe(frameDuration * 2, 0f, -slopeValue, 0),
                new Keyframe(frameDuration * 3, 0f, 0, 0),
                new Keyframe(frameDuration * 4, 0f, 0, slopeValue),
                new Keyframe(duration, 1f, slopeValue, 0)
            ),
            new AnimationCurve(
                new Keyframe(0, 0f, 0, slopeValue),
                new Keyframe(frameDuration, 1f, slopeValue, 0),
                new Keyframe(frameDuration * 2, 1f, 0, 0),
                new Keyframe(frameDuration * 3, 1f, 0, -slopeValue),
                new Keyframe(frameDuration * 4, 0f, -slopeValue, 0),
                new Keyframe(duration, 0f, 0, 0)
            ),
            new AnimationCurve(
                new Keyframe(0, 0f, 0, 0),
                new Keyframe(frameDuration, 0f, 0, slopeValue),
                new Keyframe(frameDuration * 2, 1f, slopeValue, 0),
                new Keyframe(frameDuration * 3, 1f, 0, 0),
                new Keyframe(frameDuration * 4, 1f, 0, -slopeValue),
                new Keyframe(duration, 0f, -slopeValue, 0)
            ),
            AnimationCurve.Constant(0f, duration, 1f)
        );
    }

    public static AnimationColorCurve GetPhaseInOutCurve(Color color, float durationIn, float durationOut)
    {
        return new AnimationColorCurve(
            AnimationCurve.Constant(0f, durationIn + durationOut, color.r),
            AnimationCurve.Constant(0f, durationIn + durationOut, color.g),
            AnimationCurve.Constant(0f, durationIn + durationOut, color.b),
            new AnimationCurve(
                new Keyframe(0, 0f, 0, 1f / durationIn),
                new Keyframe(durationIn, 1f, 1f / durationIn, -1f / durationOut),
                new Keyframe(durationIn + durationOut, 0f, -1f / durationOut, 0)
            )
        );
    }

    public struct FrameDelta
    {
        public required float TimeDelta;
        public required float Value;
    }

    /// <summary>
    /// Parses an array of <see cref="FrameDelta"/> to an <see cref="AnimationCurve"/>
    /// which is made of linear connections between all points of the animation. (never has curves, only straight lines)
    /// </summary>
    /// <param name="frames">The frames to parse.</param>
    /// <returns>The curve.</returns>
    public static AnimationCurve GetLinearSegmentCurve(params IEnumerable<FrameDelta> frames)
    {
        var keyframes = ListPool<Keyframe>.Shared.Rent();
        var time = 0f;
        keyframes.Add(new Keyframe(time, 0f));
        try
        {
            foreach (var delta in frames)
                keyframes.Add(new Keyframe(time += delta.TimeDelta, delta.Value));

            var previousFrame = keyframes.First();

            // set value of first frame to ensure loop correctness
            previousFrame.value = keyframes.Last().value;

            for (var i = 0; i < keyframes.Count; i++)
            {
                // get first frame instead of last to finish the stuff
                var frame = i == keyframes.Count - 1
                    ? keyframes[0]
                    : keyframes[i+1];

                var valueDelta = frame.value - previousFrame.value;
                var timeDelta = frame.time - previousFrame.time;
                var slope = valueDelta / timeDelta;
                previousFrame.outTangent = slope;
                frame.inTangent = slope;
            }

            return new AnimationCurve([.. keyframes]);
        }
        finally
        {
            ListPool<Keyframe>.Shared.Return(keyframes);
        }
    }
}
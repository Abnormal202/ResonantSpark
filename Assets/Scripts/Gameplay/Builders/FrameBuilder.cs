﻿using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class FrameBuilder : IFrameBuilder {
            public List<FrameState> frames { get; private set; }

            public FrameBuilder() {
                frames = new List<FrameState>();
            }

            public IFrameBuilder AddFrames(List<FrameState> frameList) {
                frames.AddRange(frameList);
                return this;
            }

            public FrameState[] GetFrames() {
                return frames.ToArray();
            }
        }
    }
}
﻿namespace Plotty
{
    public class SourceTarget : JumpTarget
    {
        public Source Target { get; }

        public SourceTarget(Source target)
        {
            Target = target;
        }
    }
}
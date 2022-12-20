﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpu
{
    public interface IPowerInfo
    {
        decimal CurrentPower { get; }
        decimal CurrentTargetPower { get; }

        ValueRange TargetPowerRange { get; }
    }
}

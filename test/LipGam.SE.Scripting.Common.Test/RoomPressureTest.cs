// <copyright file="RoomPressureTest.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common.Test
{
    using NSubstitute;
    using Sandbox.ModAPI.Ingame;
    using SpaceEngineers.Game.ModAPI.Ingame;
    using Xunit;

    public class RoomPressureTest
    {
        private IMyAirVent Vent1Sub { get; } = Substitute.For<IMyAirVent>();

        private IMyAirVent Vent2Sub { get; } = Substitute.For<IMyAirVent>();

        private IRoom RoomSub { get; } = Substitute.For<IRoom>();

        private RoomPressure Testee { get; }

        public RoomPressureTest()
        {
            IExtendedBlock<IMyTerminalBlock> vent1ExtendedSub = Substitute.For<IExtendedBlock<IMyTerminalBlock>>();
            vent1ExtendedSub.Block.Returns(Vent1Sub);
            IExtendedBlock<IMyTerminalBlock> vent2ExtendedSub = Substitute.For<IExtendedBlock<IMyTerminalBlock>>();
            vent2ExtendedSub.Block.Returns(Vent2Sub);
            RoomSub.Blocks.Returns(new[] { vent1ExtendedSub, vent2ExtendedSub });

            Testee = new RoomPressure();
        }

        [Fact]
        public void PressureReturnsMinimalPressureFromVents()
        {
            Vent1Sub.GetOxygenLevel().Returns(0.5f);
            Vent2Sub.GetOxygenLevel().Returns(0.8f);
            Testee.InitializeAspect(RoomSub);

            double pressure = Testee.Pressure;

            Assert.Equal(0.5, pressure);
        }

        [Fact]
        public void StatusStableWhenPressureUnchanged()
        {
            Vent1Sub.GetOxygenLevel().Returns(0.5f);
            Vent2Sub.GetOxygenLevel().Returns(0.8f);
            Testee.InitializeAspect(RoomSub);
            Testee.UpdateAspect();

            RoomPressure.PressureStatus status = Testee.Status;

            Assert.Equal(RoomPressure.PressureStatus.Stable, status);
        }

        [Fact]
        public void StatusIncreasingWhenLowestPressureIncreased()
        {
            Vent1Sub.GetOxygenLevel().Returns(0.5f);
            Vent2Sub.GetOxygenLevel().Returns(0.8f);
            Testee.InitializeAspect(RoomSub);
            Vent1Sub.GetOxygenLevel().Returns(0.6f);
            Vent2Sub.GetOxygenLevel().Returns(0.7f);
            Testee.UpdateAspect();

            RoomPressure.PressureStatus status = Testee.Status;

            Assert.Equal(RoomPressure.PressureStatus.Increasing, status);
        }

        [Fact]
        public void StatusIncreasingWhenLowestPressureDecreased()
        {
            Vent1Sub.GetOxygenLevel().Returns(0.5f);
            Vent2Sub.GetOxygenLevel().Returns(0.8f);
            Testee.InitializeAspect(RoomSub);
            Vent1Sub.GetOxygenLevel().Returns(0.4f);
            Vent2Sub.GetOxygenLevel().Returns(0.9f);
            Testee.UpdateAspect();

            RoomPressure.PressureStatus status = Testee.Status;

            Assert.Equal(RoomPressure.PressureStatus.Decreasing, status);
        }
    }
}
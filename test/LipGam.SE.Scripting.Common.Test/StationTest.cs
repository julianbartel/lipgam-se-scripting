// <copyright file="StationTest.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>
namespace LipGam.SE.Scripting.Common.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;
    using Sandbox.ModAPI.Ingame;
    using Xunit;

    public class StationTest
    {
        class FooAspect : IRoomAspect
        {
            /// <inheritdoc />
            public void InitializeAspect(IRoom room)
            {
            }

            /// <inheritdoc />
            public void UpdateAspect()
            {
            }
        }
        class OtherAspect : IRoomAspect
        {
            /// <inheritdoc />
            public void InitializeAspect(IRoom room)
            {
            }

            /// <inheritdoc />
            public void UpdateAspect()
            {
            }
        }

        private IMyGridTerminalSystem GridSub { get; } = Substitute.For<IMyGridTerminalSystem>();

        private Station Testee { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StationTest"/> class.
        /// </summary>
        public StationTest()
        {
            IMyTerminalBlock[] roomBlocks = new IMyTerminalBlock[]
                {
                    SubstituteBlock("Sec1", "Room1"),
                    SubstituteBlock("Sec1", "Room1"),
                    SubstituteBlock(string.Empty, "no-section"),
                    SubstituteBlock("Sec1", "Room2"),
                    SubstituteBlock("Sec2", "Room1"),
                    SubstituteBlock("Sec2", "Room2"),
                };
            GridSub.WhenForAnyArgs(g => g.GetBlocks(Arg.Any<List<IMyTerminalBlock>>()))
                   .Do(ci => ((List<IMyTerminalBlock>)ci[0]).AddRange(roomBlocks));
        }

        [Fact]
        public void InitializeFromGridClearsExistingRooms()
        {
            Testee = new Station((sec, name) => Substitute.For<IRoom>());
            Testee.InitializeFromGrid(GridSub);

            Testee.InitializeFromGrid(Substitute.For<IMyGridTerminalSystem>());

            Assert.Empty(Testee.Rooms);
        }


        [Fact]
        public void InitializeFromGridCreatesRooms()
        {
            Testee = new Station(
                (sec, name) =>
                    {
                        IRoom room = Substitute.For<IRoom>();
                        room.Section.Returns(sec);
                        room.Name.Returns(name);
                        return room;
                    });

            Testee.InitializeFromGrid(GridSub);

            Assert.Contains(Testee.Rooms, r => r.Section == "Sec1" && r.Name == "Room1");
            Assert.Contains(Testee.Rooms, r => r.Section == "Sec1" && r.Name == "Room2");
            Assert.Contains(Testee.Rooms, r => r.Section == "Sec2" && r.Name == "Room1");
            Assert.Contains(Testee.Rooms, r => r.Section == "Sec2" && r.Name == "Room2");
            Assert.DoesNotContain(Testee.Rooms, r => r.Name == "no-section");
        }

        [Fact]
        public void InitializeFromGridAddsBlockWithSectionToRoom()
        {
            Testee = new Station(
                (sec, name) =>
                    {
                        IRoom room = Substitute.For<IRoom>();
                        room.Section.Returns(sec);
                        room.Name.Returns(name);
                        return room;
                    });

            Testee.InitializeFromGrid(GridSub);

            Testee.Rooms.ElementAt(0).Received(2).AddBlock(Arg.Any<IExtendedBlock<IMyTerminalBlock>>());
            Testee.Rooms.ElementAt(1).Received(1).AddBlock(Arg.Any<IExtendedBlock<IMyTerminalBlock>>());
            Testee.Rooms.ElementAt(2).Received(1).AddBlock(Arg.Any<IExtendedBlock<IMyTerminalBlock>>());
            Testee.Rooms.ElementAt(3).Received(1).AddBlock(Arg.Any<IExtendedBlock<IMyTerminalBlock>>());
        }

        [Fact]
        public void InitializeFromGridAddsAspectsToRooms()
        {
            Testee = new Station(
                (sec, name) =>
                    {
                        IRoom room = Substitute.For<IRoom>();
                        room.Section.Returns(sec);
                        room.Name.Returns(name);
                        return room;
                    });
            Testee.RequiresAspect<FooAspect>();
            Testee.RequiresAspect<OtherAspect>();

            Testee.InitializeFromGrid(GridSub);

            foreach (IRoom room in Testee.Rooms)
            {
                room.Received().AddAspect(Arg.Any<FooAspect>());
                room.Received().AddAspect(Arg.Any<OtherAspect>());
            }
        }

        [Fact]
        public void RequireAspectAppliesAspectToRooms()
        {
            Testee = new Station(
                (sec, name) =>
                    {
                        IRoom room = Substitute.For<IRoom>();
                        room.Section.Returns(sec);
                        room.Name.Returns(name);
                        return room;
                    });
            Testee.InitializeFromGrid(GridSub);

            Testee.RequiresAspect<FooAspect>();

            foreach (IRoom room in Testee.Rooms)
            {
                room.Received().AddAspect(Arg.Any<FooAspect>());
            }
        }

        IMyTerminalBlock SubstituteBlock(string section, string subsection)
        {
            IMyTerminalBlock block = Substitute.For<IMyTerminalBlock>();
            block.CustomData.Returns($"Section:{section}\nSubSection:{subsection}");
            return block;
        }
    }
}
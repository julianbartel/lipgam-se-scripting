// <copyright file="RoomTest.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>
namespace LipGam.SE.Scripting.Common.Test
{
    using System;
    using System.Linq;
    using NSubstitute;
    using Sandbox.ModAPI.Ingame;
    using Xunit;

    public class RoomTest
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

        private Room Testee { get; set; }

        [Fact]
        public void SectionAndNameInitializedFromConstruction()
        {
            Testee = new Room("Section", "Name");

            Assert.Equal("Section", Testee.Section);
            Assert.Equal("Name", Testee.Name);
        }

        [Fact]
        public void GetAspectReturnsMatchingAspect()
        {
            Testee = new Room("Section", null);
            Testee.AddAspect(new FooAspect());

            Assert.NotNull(Testee.GetAspect<FooAspect>());
        }

        [Fact]
        public void GetAspectReturnsNullForUnknownAspect()
        {
            Testee = new Room("Section", null);
            Testee.AddAspect(new FooAspect());

            Assert.Null(Testee.GetAspect<OtherAspect>());
        }
        [Fact]
        public void AddingAspectInitializesIt()
        {
            Testee = new Room("Section", null);
            IRoomAspect roomAspectSub = Substitute.For<IRoomAspect>();

            Testee.AddAspect(roomAspectSub);

            roomAspectSub.Received().InitializeAspect(Testee);
        }

        [Fact]
        public void AddingAspectTwiceThrowsException()
        {
            Testee = new Room("Section", null);
            Testee.AddAspect(new FooAspect());

            Action act = () => Testee.AddAspect(new FooAspect());

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void BlocksReturnsAllAddedBlocks()
        {
            Testee = new Room("Section", null);
            ExtendedBlock<IMyTerminalBlock>[] blocks =
                {
                    new ExtendedBlock<IMyTerminalBlock>(Substitute.For<IMyTerminalBlock>()),
                    new ExtendedBlock<IMyTerminalBlock>(Substitute.For<IMyTerminalBlock>())
                };
            foreach (ExtendedBlock<IMyTerminalBlock> block in blocks)
            {
                Testee.AddBlock(block);
            }

            Assert.Equal(blocks, Testee.Blocks);
        }

        [Fact]
        public void AddingBlockTwiceThrowsException()
        {
            Testee = new Room("Section", null);
            IExtendedBlock<IMyTerminalBlock> blockSub = Substitute.For<IExtendedBlock<IMyTerminalBlock>>();
            Testee.AddBlock(blockSub);

            Action act = () => Testee.AddBlock(blockSub);

            Assert.Throws<InvalidOperationException>(act);
        }
    }
}
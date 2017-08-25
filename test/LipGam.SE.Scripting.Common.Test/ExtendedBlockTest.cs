// <copyright file="ExtendedBlockTest.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common.Test
{
    using NSubstitute;
    using Sandbox.ModAPI.Ingame;
    using Xunit;

    public class ExtendedBlockTest
    {
        public ExtendedBlockTest()
        {
            BlockSub.CustomData.Returns(string.Empty);
            Testee = new ExtendedBlock<IMyTerminalBlock>(BlockSub);
        }

        private IMyTerminalBlock BlockSub { get; } = Substitute.For<IMyTerminalBlock>();

        private ExtendedBlock<IMyTerminalBlock> Testee { get; }

        [Fact]
        public void AdditionalMetadata()
        {
            BlockSub.CustomData.Returns("SomeKey:SomeValue\nSomeOtherKey:Another value");
            Testee.ReloadData();

            Assert.Equal("SomeValue", Testee.CustomData["SomeKey"]);
            Assert.Equal("Another value", Testee.CustomData["SomeOtherKey"]);
        }

        [Fact]
        public void InnerBlockNotNull()
        {
            Assert.NotNull(Testee.Block);
        }

        [Fact]
        public void RoomInfoFromCustomData()
        {
            BlockSub.CustomData.Returns("Section:SectionName\nSubSection:SubSection Name");
            Testee.ReloadData();

            Assert.Equal("SectionName", Testee.Section);
            Assert.Equal("SubSection Name", Testee.SubSection);
        }

        [Fact]
        public void RoomInfoNullWhenUndefined()
        {
            Assert.Null(Testee.Section);
            Assert.Null(Testee.SubSection);
        }
    }
}
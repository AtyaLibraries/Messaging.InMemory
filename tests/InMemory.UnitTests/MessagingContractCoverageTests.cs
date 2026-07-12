using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory.UnitTests;

public sealed class MessagingContractCoverageTests
{
    [Fact]
    public void MessageEnvelope_With_Empty_Headers_Uses_Empty_Headers()
    {
        var envelope = new MessageEnvelope<string>(
            "message-1",
            "payload",
            headers: new Dictionary<string, string>());

        envelope.Headers.Should().BeEmpty();
    }

    [Fact]
    public void MessageEnvelope_With_Invalid_Header_Key_Throws()
    {
        var act = () => new MessageEnvelope<string>(
            "message-1",
            "payload",
            headers: new Dictionary<string, string> { [" "] = "value" });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MessageEnvelope_With_Null_Header_Value_Throws()
    {
        var act = () => new MessageEnvelope<string>(
            "message-1",
            "payload",
            headers: new Dictionary<string, string> { ["key"] = null! });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MessageEnvelope_Create_With_Whitespace_MessageId_Generates_MessageId()
    {
        var envelope = MessageEnvelope.Create(
            "payload",
            new MessagePublishOptions(messageId: " "));

        envelope.MessageId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void MessagePublishOptions_With_Empty_Headers_Uses_Empty_Headers()
    {
        var options = new MessagePublishOptions(
            headers: new Dictionary<string, string>());

        options.Headers.Should().BeEmpty();
    }
}

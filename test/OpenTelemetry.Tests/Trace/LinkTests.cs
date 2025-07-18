// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using Xunit;

namespace OpenTelemetry.Trace.Tests;

public sealed class LinkTests : IDisposable
{
    private readonly Dictionary<string, object> attributesMap = [];
    private readonly SpanContext spanContext;
    private readonly SpanAttributes tags;

    public LinkTests()
    {
        this.spanContext = new SpanContext(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.None);

        long[] longArray = [1L, 2L];
        string[] stringArray = ["a", "b"];
        bool[] boolArray = [true, false];
        double[] doubleArray = [0.1, -0.1];
        this.attributesMap.Add("MyAttributeKey0", "MyStringAttribute");
        this.attributesMap.Add("MyAttributeKey1", 10L);
        this.attributesMap.Add("MyAttributeKey2", true);
        this.attributesMap.Add("MyAttributeKey3", 0.005);
        this.attributesMap.Add("MyAttributeKey4", longArray);
        this.attributesMap.Add("MyAttributeKey5", stringArray);
        this.attributesMap.Add("MyAttributeKey6", boolArray);
        this.attributesMap.Add("MyAttributeKey7", doubleArray);
        this.tags = new SpanAttributes();
        this.tags.Add("MyAttributeKey0", "MyStringAttribute");
        this.tags.Add("MyAttributeKey1", 10L);
        this.tags.Add("MyAttributeKey2", true);
        this.tags.Add("MyAttributeKey3", 0.005);
        this.tags.Add("MyAttributeKey4", [1L, 2L]);
        this.tags.Add("MyAttributeKey5", ["a", "b"]);
        this.tags.Add("MyAttributeKey6", [true, false]);
        this.tags.Add("MyAttributeKey7", [0.1, -0.1]);
    }

    [Fact]
    public void FromSpanContext()
    {
        var link = new Link(this.spanContext);
        Assert.Equal(this.spanContext.TraceId, link.Context.TraceId);
        Assert.Equal(this.spanContext.SpanId, link.Context.SpanId);
    }

    [Fact]
    public void FromSpanContext_WithAttributes()
    {
        var link = new Link(this.spanContext, this.tags);
        Assert.Equal(this.spanContext.TraceId, link.Context.TraceId);
        Assert.Equal(this.spanContext.SpanId, link.Context.SpanId);

        Assert.NotNull(link.Attributes);
        foreach (var attributemap in this.attributesMap)
        {
            Assert.Equal(attributemap.Value, link.Attributes!.FirstOrDefault(a => a.Key == attributemap.Key).Value);
        }
    }

    [Fact]
    public void Equality()
    {
        var link1 = new Link(this.spanContext);
        var link2 = new Link(this.spanContext);
        object link3 = new Link(this.spanContext);

        Assert.Equal(link1, link2);
        Assert.True(link1 == link2);
        Assert.True(link1.Equals(link3));
    }

    [Fact]
    public void NotEquality()
    {
        var link1 = new Link(new SpanContext(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.None));
        var link2 = new Link(new SpanContext(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.None));

        Assert.NotEqual(link1, link2);
        Assert.True(link1 != link2);
    }

    [Fact]
    public void NotEquality_WithAttributes()
    {
        var tag1 = new SpanAttributes();
        var tag2 = this.tags;
        var link1 = new Link(this.spanContext, tag1);
        var link2 = new Link(this.spanContext, tag2);

        Assert.NotEqual(link1, link2);
        Assert.True(link1 != link2);
    }

    [Fact]
    public void TestGetHashCode()
    {
        var link1 = new Link(this.spanContext, this.tags);
        Assert.NotEqual(0, link1.GetHashCode());
    }

    public void Dispose()
    {
        Activity.Current = null;
        GC.SuppressFinalize(this);
    }
}

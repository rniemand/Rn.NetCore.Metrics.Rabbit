using NUnit.Framework;
using Rn.NetCore.Metrics.Rabbit.Config;

namespace Rn.NetCore.Metrics.Rabbit.T1.Tests.Config;

[TestFixture]
public class RabbitOutputConfigTests
{
  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Enabled()
  {
    Assert.IsFalse(new RabbitOutputConfig().Enabled);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Username()
  {
    Assert.AreEqual("guest", new RabbitOutputConfig().Username);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Password()
  {
    Assert.AreEqual("guest", new RabbitOutputConfig().Password);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_VirtualHost()
  {
    Assert.AreEqual("/", new RabbitOutputConfig().VirtualHost);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Host()
  {
    Assert.AreEqual("127.0.0.1", new RabbitOutputConfig().Host);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Port()
  {
    Assert.AreEqual(5672, new RabbitOutputConfig().Port);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_Exchange()
  {
    Assert.AreEqual("amq.topic", new RabbitOutputConfig().Exchange);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_RoutingKey()
  {
    Assert.AreEqual("rn_core.metrics", new RabbitOutputConfig().RoutingKey);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_BackOffTimeSec()
  {
    Assert.AreEqual(15, new RabbitOutputConfig().BackOffTimeSec);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_CoolDownTimeSec()
  {
    Assert.AreEqual(300, new RabbitOutputConfig().CoolDownTimeSec);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_CoolDownThreshold()
  {
    Assert.AreEqual(3, new RabbitOutputConfig().CoolDownThreshold);
  }

  [Test]
  public void RabbitOutputConfig_Given_Constructed_ShouldDefault_MaxCoolDownRuns()
  {
    Assert.AreEqual(0, new RabbitOutputConfig().MaxCoolDownRuns);
  }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Metrics.Rabbit.Config;
using Rn.NetCore.Metrics.Rabbit.Models;

namespace Rn.NetCore.Metrics.Rabbit;

public interface IRabbitConnection
{
  void Configure(RabbitOutputConfig config);
  Task SubmitPoint(LineProtocolPoint point);
  Task SubmitPoints(List<LineProtocolPoint> points);
}
// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;

using microservice_map_info.Protos;

var channel = GrpcChannel.ForAddress(new Uri("https://localhost:7175"));
var client = new DistanceInfo.DistanceInfoClient(channel);

var response = await client.GetDistanceAsync(new Cities { OriginCity = "Dallas,Tx", DestinationCity = "Los%20Angeles,CA" });

Console.WriteLine(response.Miles);
Console.ReadKey();

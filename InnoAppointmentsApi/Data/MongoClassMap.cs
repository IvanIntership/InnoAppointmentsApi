using InnoAppointmentsApi.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace InnoAppointmentsApi.Data;

public static class MongoClassMap
{
    public static void Register()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Schedule)))
        {
            BsonClassMap.RegisterClassMap<Schedule>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetIdGenerator(GuidGenerator.Instance);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(WorkDay)))
        {
            BsonClassMap.RegisterClassMap<WorkDay>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}
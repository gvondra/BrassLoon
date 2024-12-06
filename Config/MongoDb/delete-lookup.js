db = db.getSiblingDB("dev-bl-config");
printjson(db.LookupHistory.deleteMany({}));
printjson(db.Lookup.deleteMany({}));
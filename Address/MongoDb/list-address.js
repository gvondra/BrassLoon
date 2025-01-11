db = db.getSiblingDB("dev-bl-address");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("address")
var address = db.Address.find();
printjson(address);
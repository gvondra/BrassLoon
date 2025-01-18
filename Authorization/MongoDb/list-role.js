db = db.getSiblingDB("dev-bl-auth");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("Role")
var item = db.Role.find();
printjson(item);
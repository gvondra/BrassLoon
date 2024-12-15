db = db.getSiblingDB("dev-bl-account");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("User")
var user = db.User.find();
printjson(user);
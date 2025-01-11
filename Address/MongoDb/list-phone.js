db = db.getSiblingDB("dev-bl-address");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("phone")
var phone = db.Phone.find();
printjson(phone);
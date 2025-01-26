db = db.getSiblingDB("dev-bl-work-task");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("WorkGroup")
var group = db.WorkGroup.find();
printjson(group);
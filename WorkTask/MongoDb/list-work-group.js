db = db.getSiblingDB("dev-bl-work-task");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("WorkGroup")
var group = db.WorkGroup.find({ _id: UUID('a0ea12b8-de61-4b07-97c6-7b08332619ca') });
printjson(group);
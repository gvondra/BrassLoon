db = db.getSiblingDB("dev-bl-work-task");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("WorkTaskTypeGroup")
var taskTypeGroup = db.WorkTaskTypeGroup.find();
printjson(taskTypeGroup);
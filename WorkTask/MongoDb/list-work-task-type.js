db = db.getSiblingDB("dev-bl-work-task");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("WorkTaskType")
var taskType = db.WorkTaskType.find();
printjson(taskType);
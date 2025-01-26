db = db.getSiblingDB("dev-bl-work-task");
print("WorkTaskType")
var taskType = db.WorkTaskType.find({ "Title": /^TestClient\s*Generated\s*20\d{2}/});
printjson(taskType);
db.WorkTaskType.deleteMany({ "Title": /^TestClient\s*Generated\s*20\d{2}/});
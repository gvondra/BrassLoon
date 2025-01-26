db = db.getSiblingDB("dev-bl-work-task");
print("WorkGroup")
var group = db.WorkGroup.find({ "Title": /^TestClient\s*Generated\s*20\d{2}/});
printjson(group);
db.WorkGroup.deleteMany({ "Title": /^TestClient\s*Generated\s*20\d{2}/});
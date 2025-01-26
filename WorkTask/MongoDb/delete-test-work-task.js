db = db.getSiblingDB("dev-bl-work-task");
var tasks = db.WorkTask.find({ "Title": /^TestClient\s*Generated\s*20\d{2}/}).toArray();
print(`Deleting ${tasks.length} documents`);
for (const task of tasks) {    
    print(`Deleting ${task["Title"]}`)
    db.WorkTaskComment.deleteMany({ "WorkTaskId": task['_id'] });
    db.WorkTaskContext.deleteMany({ "WorkTaskId": task['_id'] });
    db.WorkTask.deleteOne({ _id: task['_id'] });
}
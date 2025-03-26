db = db.getSiblingDB("dev-bl-work-task");
db.WorkGroup.updateOne(
    { _id: UUID('a0ea12b8-de61-4b07-97c6-7b08332619ca') },
    { $set: { Members: [] } });
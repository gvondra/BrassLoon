# About Log Purge

This console application will purge log entries older than a configurable age.

### Worker

A worker is an organizational unit that allows asynchronous processing per account domain.

### Meta Data

The process uses a set of meta data tables to identify records to delete, when to delete, and the delete status.

## Initialize Workers

This process deletes workers that were last updated more than 36 hours ago and have a completed or error status.  Then, all incomplete workers are reset to an unprocessed status.  Finally, it creates new workers for domains not having a worker.

## Purge Meta Data

From each meta data table, delete records in a complete or error status that were last update more than a configured amount of time in the past. Configuration key "PurgeMetaDataTimespan" determines the period of time.

## Claim Worker

An application process claim's a worker to prevent other processes from claiming the same work.

## Purge

1. Update meta data

   For each target table record created before the configured period of time, create a new meta data record.  The meta data record expiration is set to the begining of the next month.  Newly identified target records are not deleted until next month.  Also, error records are reset to be reprocessed.

2. Delete records

3. Change worker status to complete or error
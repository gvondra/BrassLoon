import argparse
import concurrent.futures
from datetime import datetime
import logging
import requests
import sys

BASE_URL = "http://localhost:5000/api/"

def StartCreate(domain, count):
    with requests.Session() as session:
        with concurrent.futures.ThreadPoolExecutor(max_workers=8) as executor:
            executor.map(Create, [domain] * count, range(count), [session] * count)

def Create(domain, index, session):
    try:
        trace = {
            "DomainId": domain,
            "EventCode": "trace-py-test-script",
            "Message": "message {}".format(index),
            "Data": {
                "timestamp": datetime.now().isoformat()
            }
        }
        response = session.post("{0}Trace".format(BASE_URL), json=trace, verify=False)
        if response.status_code != 200:
            logging.info("Create trace status {}".format(response.status_code))
            logging.info(response.text)
    except:
        logging.error(sys.exc_info()[0])            
        raise

def GetArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument("--domain", required=True)
    parser.add_argument("--count", default=1)
    parser.add_argument("--log", default="info")
    return parser.parse_args()

if __name__ == "__main__":    
    args = GetArgs()
    logLevel = getattr(logging, args.log.upper())
    logging.basicConfig(level=logLevel)
    StartCreate(args.domain, int(args.count))
    logging.info("Complete")
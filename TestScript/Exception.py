import argparse
import ClientCredentialToken
import concurrent.futures
from datetime import datetime
import logging
import requests
import sys

BASE_URL = "http://localhost:5001/api/"

def StartCreate(domain, count, tkn):
    with requests.Session() as session:
        with concurrent.futures.ThreadPoolExecutor(max_workers=8) as executor:
            executor.map(Create, [domain] * count, range(count), [session] * count, [tkn] * count)

def Create(domain, index, session, tkn):
    try:
        exception = {
            "DomainId": domain,
            "Message": "test exception",
            "Data": {
                "timestamp": datetime.now().isoformat()
            }
        }
        response = session.post("{0}Exception".format(BASE_URL), json=exception, verify=False, headers={"Authorization": "Bearer {}".format(tkn)})
        if response.status_code != 200:
            logging.info("Create exception status {}".format(response.status_code))
            logging.info(response.text)
    except:
        logging.error(sys.exc_info()[0])            
        logging.error(sys.exc_info()[1])           
        raise

def GetArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument("--domain", required=True)
    parser.add_argument("--count", default=1)
    parser.add_argument("--client-id", required=True)
    parser.add_argument("--secret", required=True)
    parser.add_argument("--log", default="info")
    return parser.parse_args()

if __name__ == "__main__":    
    args = GetArgs()
    logLevel = getattr(logging, args.log.upper())
    logging.basicConfig(level=logLevel)
    tkn = ClientCredentialToken.Create(args.client_id, args.secret)
    StartCreate(args.domain, int(args.count), tkn)
    logging.info("Complete")
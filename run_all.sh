
#!/bin/bash

# Navigate to the solution directory
cd "$(dirname "$0")"

# Build and run all services
docker-compose up --build

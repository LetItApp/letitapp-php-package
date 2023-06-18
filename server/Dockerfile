# Use a Node.js 16 LTS base image
FROM node:16-alpine

# Set the working directory inside the container
WORKDIR /app

# Copy the source code into the container
COPY ./src /app

# Install dependencies
RUN npm install

# Set the entrypoint to index.js
CMD ["node", "index.js"]
# Use Node.js as base image
FROM node:latest

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY package*.json ./

# Install dependencies using npm
RUN npm install

# Copy the rest of the application code
COPY . .

EXPOSE 5173

# Start server
CMD ["npm", "run", "dev"]

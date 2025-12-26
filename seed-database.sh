#!/bin/bash

# Script to seed the database with demo data
# Usage: ./seed-database.sh [--clear]
#   --clear  : Clear existing data before seeding

echo "🌱 Seeding database with demo data..."
cd "$(dirname "$0")/DatabaseSeeder"

if [ "$1" == "--clear" ] || [ "$1" == "-c" ]; then
    echo "⚠️  Clear mode enabled - existing data will be removed!"
    dotnet run -- --clear
else
    dotnet run
fi

exit_code=$?

if [ $exit_code -eq 0 ]; then
    echo ""
    echo "✅ Database seeding completed successfully!"
else
    echo ""
    echo "❌ Database seeding failed with exit code: $exit_code"
    exit $exit_code
fi


#!/usr/bin/env bash
# Revitalize SaaS Management CLI - Sprint121
# Usage: ./revitalize-cli.sh [command] [options]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Help function
show_help() {
    cat << EOF
Revitalize SaaS Management CLI - Sprint121

USAGE:
    ./revitalize-cli.sh [COMMAND] [OPTIONS]

COMMANDS:
    tenant create <name> <code>    Create a new tenant
    tenant list                    List all tenants
    tenant activate <id>           Activate a tenant
    tenant deactivate <id>         Deactivate a tenant
    
    service create <tenant-id>     Create a new service request
    service list <tenant-id>       List service requests for tenant
    service assign <id> <tech-id>  Assign technician to service request
    
    tech create <tenant-id>        Create a new technician profile
    tech list <tenant-id>          List technicians for tenant
    tech activate <id>             Activate a technician
    
    platform status                Show platform status
    platform migrate               Run database migrations
    platform seed                  Seed sample data
    
    build                          Build the Revitalize module
    test                           Run Revitalize tests
    deploy                         Deploy Revitalize updates

OPTIONS:
    -h, --help                     Show this help message
    -v, --verbose                  Enable verbose output
    --dry-run                      Show what would be done without executing

EXAMPLES:
    ./revitalize-cli.sh tenant create "Acme Plumbing" "ACME"
    ./revitalize-cli.sh service list 1
    ./revitalize-cli.sh platform status

EOF
}

# Platform status
platform_status() {
    log_info "Checking Revitalize platform status..."
    
    # Check if project builds
    if cd "$PROJECT_ROOT" && dotnet build --no-restore > /dev/null 2>&1; then
        log_success "✓ Project builds successfully"
    else
        log_error "✗ Project build failed"
        return 1
    fi
    
    # Check configuration
    if [ -f "$PROJECT_ROOT/appsettings.json" ]; then
        if grep -q "Revitalize" "$PROJECT_ROOT/appsettings.json"; then
            log_success "✓ Revitalize configuration found"
        else
            log_warning "⚠ Revitalize configuration missing"
        fi
    fi
    
    # Check services registration
    if grep -q "Revitalize.Services" "$PROJECT_ROOT/Program.cs"; then
        log_success "✓ Revitalize services registered"
    else
        log_warning "⚠ Revitalize services not registered"
    fi
    
    log_info "Platform status check complete"
}

# Build function
build_revitalize() {
    log_info "Building Revitalize module..."
    cd "$PROJECT_ROOT"
    
    if dotnet build --configuration Release; then
        log_success "Revitalize module built successfully"
    else
        log_error "Build failed"
        return 1
    fi
}

# Test function
test_revitalize() {
    log_info "Running Revitalize tests..."
    cd "$PROJECT_ROOT"
    
    # Check if test projects exist
    if find . -name "*.Tests.csproj" -type f | head -1 > /dev/null; then
        if dotnet test --no-build --verbosity minimal; then
            log_success "All tests passed"
        else
            log_error "Some tests failed"
            return 1
        fi
    else
        log_warning "No test projects found"
    fi
}

# Seed sample data
seed_data() {
    log_info "Seeding Revitalize sample data..."
    
    # This would normally call into the application to seed data
    # For now, just create some documentation
    cat > "$PROJECT_ROOT/revitalize-sample-data.json" << EOF
{
  "tenants": [
    {
      "companyName": "Demo Plumbing Co",
      "tenantCode": "DEMO",
      "description": "Sample tenant for demonstration purposes"
    }
  ],
  "serviceRequests": [
    {
      "title": "Kitchen Sink Repair",
      "description": "Leaky faucet needs repair",
      "serviceType": "Plumbing",
      "priority": "Medium",
      "customerName": "John Doe",
      "customerPhone": "555-0123"
    }
  ]
}
EOF
    
    log_success "Sample data template created: revitalize-sample-data.json"
}

# Main command dispatcher
main() {
    if [ $# -eq 0 ]; then
        show_help
        exit 0
    fi
    
    case "$1" in
        "tenant")
            case "$2" in
                "create")
                    log_info "Creating tenant: $3 ($4)"
                    ;;
                "list")
                    log_info "Listing all tenants..."
                    ;;
                *)
                    log_error "Unknown tenant command: $2"
                    exit 1
                    ;;
            esac
            ;;
        "platform")
            case "$2" in
                "status")
                    platform_status
                    ;;
                "seed")
                    seed_data
                    ;;
                *)
                    log_error "Unknown platform command: $2"
                    exit 1
                    ;;
            esac
            ;;
        "build")
            build_revitalize
            ;;
        "test")
            test_revitalize
            ;;
        "-h"|"--help")
            show_help
            ;;
        *)
            log_error "Unknown command: $1"
            show_help
            exit 1
            ;;
    esac
}

# Run main function with all arguments
main "$@"
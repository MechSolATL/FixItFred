# Copilot Productivity Guidelines

## Architecture
- Follow the MVP-Core architecture principles.
- Ensure modular design with clear separation of concerns.
- Use namespaces consistently to avoid conflicts.

## Workflows
- Implement CI/CD pipelines for automated testing and deployment.
- Use feature branches for development and merge via pull requests.

## Integration Rules
- Integrate third-party services only after security review.
- Document all API integrations in the `Docs/Blueprints` folder.

## Commit Protocols
- Use semantic commit messages (e.g., `feat: add new validation logic`).
- Include references to Jira tickets or sprint numbers in commit messages.

## Refactoring
- Prioritize readability and maintainability.
- Remove unused code and dependencies.

## Patch Logic
- Ensure patches are backward-compatible.
- Test patches in staging before production deployment.

## Scaffolding
- Use templates for new Razor Pages and services.
- Follow naming conventions outlined in `Docs/Blueprints`.

## Code Suggestions
- Reference `.github/copilot-instructions.md` for all future code suggestions.
- Ensure compliance with architecture and workflows.

## Documentation
- Update `README.md` and `CHANGELOG.md` with every major change.
- Use inline comments for complex logic.

## Security
- Follow OWASP guidelines for secure coding.
- Use parameterized queries to prevent SQL injection.

## Testing
- Write unit tests for all new features.
- Achieve 80% code coverage before merging.

---

**Last Updated:** July 30, 2025

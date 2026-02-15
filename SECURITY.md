# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version   | Supported          |
| --------- | ------------------ |
| Latest    | :white_check_mark: |
| < Latest  | :x:                |

## Reporting a Vulnerability

We take the security of VersionMark seriously. If you believe you have found a
security vulnerability, please report it to us as described below.

### How to Report

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them using one of the following methods:

- **Preferred**: [GitHub Security Advisories][security-advisories] - Use the private vulnerability reporting feature
- **Alternative**: Contact the project maintainers directly through GitHub

Please include the following information in your report:

- **Type of vulnerability** (e.g., SQL injection, cross-site scripting, etc.)
- **Full path** of source file(s) related to the vulnerability
- **Location** of the affected source code (tag/branch/commit or direct URL)
- **Step-by-step instructions** to reproduce the issue
- **Proof-of-concept or exploit code** (if possible)
- **Impact** of the issue, including how an attacker might exploit it

### What to Expect

After submitting a vulnerability report, you can expect:

1. **Acknowledgment**: We will acknowledge receipt of your vulnerability report promptly
2. **Investigation**: We will investigate the issue and determine its impact and severity
3. **Updates**: We will keep you informed of our progress as we work on a fix
4. **Resolution**: Once the vulnerability is fixed, we will:
   - Release a security patch
   - Publicly disclose the vulnerability (with credit to you, if desired)
   - Update this security policy as needed

### Response Timeline

- **Initial Response**: Promptly
- **Status Update**: Regular updates as investigation progresses
- **Fix Timeline**: Varies based on severity and complexity

### Security Update Policy

Security updates will be released as:

- **Critical vulnerabilities**: Patch release as soon as possible
- **High severity**: Patch release within 30 days
- **Medium/Low severity**: Included in the next regular release

## Security Best Practices

When using VersionMark, we recommend following these security best practices:

### Input Validation

- Validate configuration files (.versionmark.yaml) before processing
- Be cautious when using untrusted configuration files
- Use the latest version of VersionMark to benefit from security updates

### Dependencies

- Keep VersionMark and its dependencies up to date
- Review the release notes for security-related updates
- Use `dotnet list package --vulnerable` to check for vulnerable dependencies

### Execution Environment

- Run VersionMark with the minimum required permissions
- Avoid running VersionMark as a privileged user unless necessary
- Review configuration files for potentially dangerous commands before execution

## Known Security Considerations

### Command Execution

VersionMark executes shell commands defined in the configuration file to capture version
information. Users should:

- Review the `.versionmark.yaml` configuration file before use
- Be cautious when using configuration files from untrusted sources
- Understand that commands are executed with the same permissions as the VersionMark process
- Validate that commands in the configuration file are safe to execute
- Avoid executing commands that could modify system state or access sensitive data

### File System Access

VersionMark reads and writes files on the local file system. Users should:

- Ensure appropriate file permissions are set on output files (JSON and markdown)
- Be cautious when writing files in shared directories
- Validate that glob patterns used in publish mode do not inadvertently process
  sensitive files
- Review generated markdown files before committing to version control

## Security Disclosure Policy

When we receive a security bug report, we will:

1. Confirm the problem and determine affected versions
2. Audit code to find similar problems
3. Prepare fixes for all supported versions
4. Release patches as soon as possible

We will credit security researchers who report vulnerabilities responsibly. If you would like to be credited:

- Provide your name or pseudonym
- Optionally provide a link to your website or GitHub profile
- Let us know if you prefer to remain anonymous

## Third-Party Dependencies

VersionMark relies on third-party packages. We:

- Regularly update dependencies to address known vulnerabilities
- Use Dependabot to monitor for security updates
- Review security advisories for all dependencies

To check for vulnerable dependencies yourself:

```bash
dotnet list package --vulnerable
```

## Contact

For security concerns, please use [GitHub Security Advisories][security-advisories] or contact the project
maintainers directly through GitHub.

For general bugs and feature requests, please use [GitHub Issues][issues].

## Additional Resources

- [OWASP Secure Coding Practices][owasp-practices]
- [.NET Security Best Practices][dotnet-security]
- [GitHub Security Advisories][security-advisories]

Thank you for helping keep VersionMark and its users safe!

[security-advisories]: https://github.com/demaconsulting/VersionMark/security/advisories
[issues]: https://github.com/demaconsulting/VersionMark/issues
[owasp-practices]: https://owasp.org/www-project-secure-coding-practices-quick-reference-guide/
[dotnet-security]: https://learn.microsoft.com/en-us/dotnet/standard/security/

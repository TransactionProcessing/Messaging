# MessagingService HTTPS rollout

MessagingService keeps its existing HTTP listener on port `5006` while HTTPS is
introduced on port `5007`. This lets callers move to HTTPS before HTTP is
removed. The existing SMTP2GO and The SMS Works endpoints are unrelated and
remain unchanged.

## Configure the HTTPS listener

Set these environment variables in the development or staging service
environment. The certificate path is the activation switch: when it is absent,
the service uses its existing HTTP configuration. When present, the service
binds both its existing HTTP listener and the HTTPS listener:

```text
MessagingService__Https__CertificatePath=<path to mounted CA-issued PFX>
MessagingService__Https__CertificatePassword=<secret-store value>
```

Mount the CA-issued server certificate into the service environment and provide
its password through the deployment secret store. Do not add the certificate,
private key, or password to this repository or a container image. The
certificate must be valid for the hostname callers use, and callers must trust
its issuing CA.

Do not set the certificate path until a certificate has been issued and
mounted. The certificate password may be empty if the PFX has no password.
Without the certificate path, the service continues to start on its existing
HTTP listener and port.

The container images no longer import repository certificate files or PFX
files. Provision the issuing CA trust into the host/runtime trust store through
the deployment environment so health checks and callers can validate the
certificate chain.

## Health checks and rollout

The existing health URL remains `http://localhost:5006/healthui` during the
transition. Once the certificate and CA trust are installed, probe
`https://localhost:5007/healthui` from a host that trusts the issuing CA. Also
verify that a client with an untrusted root and a client using a hostname that
does not match the certificate both reject the HTTPS connection.

Keep the current HTTP health check and listener until callers and deployment
health checks have been verified over HTTPS. The outbound SecurityService
migration is tracked separately in Messaging issue #458.


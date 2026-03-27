-- ============================================================
-- Email confirmation & password reset
-- ============================================================

ALTER TABLE usr ADD COLUMN is_email_confirmed BOOLEAN NOT NULL DEFAULT FALSE;

CREATE TABLE email_token (
    email_token_id BIGINT    GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usr_id         INT       NOT NULL REFERENCES usr (usr_id) ON DELETE CASCADE,
    token_hash     VARCHAR(255) NOT NULL UNIQUE,
    token_type     SMALLINT  NOT NULL, -- 0 = EmailConfirmation, 1 = PasswordReset
    expires_at     TIMESTAMP NOT NULL,
    created_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_email_token_usr ON email_token (usr_id);

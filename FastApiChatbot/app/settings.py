from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    dotnet_base_url: str = "https://localhost:7221"
    dotnet_verify_ssl: bool = False
    request_timeout_seconds: int = 30

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore",
    )


settings = Settings()

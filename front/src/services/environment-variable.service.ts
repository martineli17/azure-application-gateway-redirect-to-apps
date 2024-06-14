export async function getEnvironmentVariableAsync(
  endpoint: string
): Promise<any> {
  const response = await fetch(`${endpoint}`, {
    method: "GET",
  });

  return (await response.json()) ?? {};
}

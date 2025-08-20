import { StreamTokenProvider } from './types'

export class ApiClient {
  private readonly baseUrl: string
  private readonly tokenProvider: StreamTokenProvider

  constructor(baseUrl: string, tokenProvider: StreamTokenProvider) {
    this.baseUrl = baseUrl.replace(/\/$/, '')
    this.tokenProvider = tokenProvider
  }

  private async authHeaders(): Promise<HeadersInit> {
    const token = await this.tokenProvider()
    return {
      Authorization: `Bearer ${token.accessToken}`
    }
  }

  async postSdp(pathOrUrl: string, sdp: string): Promise<Response> {
    const headers: HeadersInit = {
      ...(await this.authHeaders()),
      'Content-Type': 'application/sdp'
    }
    const url = pathOrUrl.startsWith('http') ? pathOrUrl : `${this.baseUrl}${pathOrUrl}`
    return fetch(url, { method: 'POST', headers, body: sdp })
  }

  async delete(pathOrUrl: string): Promise<Response> {
    const headers: HeadersInit = {
      ...(await this.authHeaders())
    }
    const url = pathOrUrl.startsWith('http') ? pathOrUrl : `${this.baseUrl}${pathOrUrl}`
    return fetch(url, { method: 'DELETE', headers })
  }

  async postJson<TIn extends object, TOut = unknown>(path: string, body: TIn): Promise<TOut> {
    const headers: HeadersInit = {
      ...(await this.authHeaders()),
      'Content-Type': 'application/json'
    }
    const res = await fetch(`${this.baseUrl}${path}`, { method: 'POST', headers, body: JSON.stringify(body) })
    if (!res.ok) throw new Error(`POST ${path} failed: ${res.status}`)
    return (await res.json()) as TOut
  }
}



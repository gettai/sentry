# Tai Sentry

一个提供在Windows平台统计软件使用时长能力的后台程序。使用了WebSocket作为服务端，给订阅端提供数据推送服务。程序本体是后台进程，没有UI界面，只提供了软件使用时长统计的基本功能，且不存储数据，需要开发者自行编写订阅端软件接收和处理数据。

## 特点

- 离开监听，当用户离开电脑或休眠时，Tai Sentry能够监测到并停止计时直到用户回来，订阅端无需处理。
- 计算时长，Tai Sentry使用了计时器统计时长，不会受到本地日期时间更改影响，也不需要订阅端手动计算。

订阅端软件只需要专注于处理统计数据的储存和展示即可。

## 如何对接

1. 使用你熟悉的语言编写订阅端；
2. 通过订阅端启动 Tai Sentry；
3. 使用 `WebSocket` 连接Tai Sentry，默认连接地址是： `ws://127.0.0.1:21123/TaiSentry`；
4. 监听 WebSocket 的消息事件接收软件时长统计数据。

## 启动参数

```
-wspath:PATH 自定义WebSocket服务端Path
-wsport:PORT 自定义WebSocket服务端端口

-activedata 接收软件焦点切换数据
```

## 终止进程

当Tai Sentry进程启动后只会在后台存活 `60` 秒，在此时间内没有订阅端连接将会自动终止进程。其他情况需要终止由订阅端自行处理。

## 数据结构

默认情况下，Tai Sentry仅会推送应用时长、用户状态两种数据消息。消息以 `JSON` 格式推送。

#### 消息统一结构

```JSON
{
	"Type": "String",
	"Msg": "String",
	"CreateTime": TimeStamp
}
```

#### 应用时长数据结构

```JSON
{
	"Type": "AppData",
	"Msg": "{\"Duration\":2,\"ActiveTime\":\"2024-04-28T18:57:19.9513484+08:00\",\"EndTime\":\"2024-04-28T18:57:22.4910219+08:00\",\"App\":{\"PID\":24420,\"Type\":0,\"Process\":\"Tai\",\"Description\":\"Tai\",\"ExecutablePath\":\"E:\\\\gettai\\\\tai\\\\Tai\\\\bin\\\\Debug\\\\net6.0-windows\\\\Tai.exe\"},\"Window\":{\"ClassName\":\"HwndWrapper[Tai;;34e9615b-67f2-42bb-b032-930cd92391c5]\",\"Title\":\"MainWindow\",\"Handle\":{\"value\":7737414},\"Width\":800,\"Height\":450,\"X\":1109,\"Y\":148}}",
	"CreateTime": 1714301842
}
```

#### 用户状态数据结构

```JSON
{
	"Type": "Status",
	"Msg": "Depart",
	"CreateTime": 1714305358
}
```

#### 属性说明

| 属性 | 类型 | 可选值 | 说明 |
| --- | --- | --- | --- |
| Type | String | System,Status,AppData,ActiveData | 消息类型 |
| Msg | String | - | 消息内容 |
| CreateTime | TimeStamp | - | 消息推送时间戳 |

`Type`

| 值 | 说明 | Msg内容 | 说明 |
| --- | --- | --- | --- |
| System | 系统消息 | - | - |
| Status | 用户状态消息 | Active,Depart | Active:活跃,Depart:离开|
| AppData | APP时长消息 | 见上方数据结构 | - |
| ActiveData | 焦点切换消息 | - | 仅在带有`-activedata`参数启动时推送 |

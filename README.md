# Unity 相机行为编辑器 (Camera Behavior Editor)

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Cinemachine](https://img.shields.io/badge/Cinemachine-2.8%2B-orange.svg)](https://unity.com/packages/essentials/cinemachine)

一个功能强大的Unity相机行为系统，提供可视化节点编辑器和完整的相机状态管理解决方案。基于Cinemachine构建，支持复杂的相机动作序列、多种虚拟相机切换和丰富的相机行为。

## ✨ 核心特性

### 🎯 可视化节点编辑器
- **相机行为树编辑器**：基于Unity GraphView的可视化节点编辑系统
- **相机行为模板编辑器**：创建和管理可复用的相机行为模板
- **实时预览**：编辑器内直接测试和调试相机行为
- **拖拽式操作**：直观的节点连接和参数配置

### 🔄 状态机系统
- **CMCameraFSM**：专业的相机有限状态机，管理各种相机状态转换
- **多种内置状态**：
  - `AutoState` - 自动跟随状态
  - `ChaseBackState` - 追背状态  
  - `RotateToAimState` - 瞄准旋转状态
  - `ZoomFromToState` - 视距缓动状态
  - `ViewChangeState` - 视角切换状态
  - `SuspendState` - 挂起状态

### 📹 多样化虚拟相机
- **FreeLook相机**：自由视角相机，支持360度旋转和缩放
- **瞬切相机 (CutCamera)**：无过渡的即时相机切换
- **缓切相机 (BlendCamera)**：平滑过渡的相机切换
- **2D/3D视角切换**：动态切换不同相机模式

### 🎬 高级相机功能
- **相机震动系统**：基于Cinemachine Impulse的真实震动效果
- **智能追踪**：角色跟随和目标锁定
- **视距控制**：动态缩放和视野调整
- **阻尼系统**：可配置的相机跟随平滑度
- **渲染层管理**：动态控制相机可见层级

### ⚡ 性能优化
- **对象池系统**：高效的内存管理和对象复用
- **时间回调系统**：精确的定时任务管理
- **单例模式**：统一的相机管理入口

## 🏗️ 系统架构

```
CMCameraManager (核心管理器)
├── FSM System (状态机系统)
│   ├── CMCameraFSM (相机状态机)
│   ├── CameraStates (各种相机状态)
│   └── CameraEvents (相机事件)
├── CameraTree System (行为树系统)
│   ├── CameraTree (行为树资源)
│   ├── CameraNodes (各种行为节点)
│   └── StateDrivenCamera (状态驱动相机)
├── Editor Tools (编辑器工具)
│   ├── CameraTreeEditor (行为树编辑器)
│   ├── CameraBehaviourEditor (行为模板编辑器)
│   └── Visual Components (可视化组件)
└── Support Systems (支持系统)
    ├── ObjectPool (对象池)
    ├── TimeCallback (时间回调)
    └── Extensions (扩展工具)
```

## 📦 安装说明

### 前置依赖
- Unity 2022.3 或更高版本
- Cinemachine 2.8.0 或更高版本

### 安装步骤
1. 将整个 `CameraFSM` 文件夹复制到您的项目 `Assets` 目录下
2. 确保已安装 Cinemachine 包：
   ```
   Window -> Package Manager -> Cinemachine -> Install
   ```
3. 在场景中放置 `CameraRoot.prefab` 预制体
4. 配置相机目标和基础参数

## 🚀 快速开始

### 1. 基础设置
```csharp
// 获取相机管理器实例
var cameraManager = CMCameraManager.Instance;

// 设置跟随和注视目标
cameraManager.SetFllowAndLookAt(playerTransform);

// 切换到自动状态
cameraManager.ToAutoState();
```

### 2. 执行相机行为
```csharp
// 执行相机行为树事件 (使用预配置的行为节点)
CMCameraManager.Instance.BeginCameraTreeActionEvent(1001);

// 执行相机追背 (使用行为模板)
CMCameraManager.Instance.StartChaseBack(3001, waitTime: 0.5f, recentTime: 2.0f);

// 执行视距缓动动画
CMCameraManager.Instance.StartZoomFromToEvent(3006);
```

### 3. 虚拟相机切换
```csharp
// 打开瞬切相机
Vector3 cutPosition = new Vector3(10, 5, 0);
Quaternion cutRotation = Quaternion.LookRotation(Vector3.forward);
CMCameraManager.Instance.OpenCutVCamera(cutPosition, cutRotation);

// 关闭瞬切相机
CMCameraManager.Instance.CloseCutVCamera();

// 使用缓切相机 (带平滑过渡)
CMCameraManager.Instance.OpenBlendVCamera(blendPosition, blendRotation);
```

### 4. 相机震动效果
```csharp
// 播放震动效果
float duration = 1.0f;           // 震动持续时间
float sensitivity = 0.5f;        // 震动敏感度
CMCameraManager.Instance.PlayShake(duration, sensitivity);

// 停止震动
CMCameraManager.Instance.StopShake();
```

## 🎨 编辑器使用

### 相机行为树编辑器
1. 在Project窗口中右键选择 `Create -> 相机节点图`
2. 双击创建的CameraTree资源打开编辑器
3. 右键添加各种相机行为节点
4. 连接节点创建复杂的相机动作序列
5. 配置每个节点的参数和触发条件

### 相机行为模板编辑器  
1. 在Project窗口中右键选择 `Create -> 相机行为/行为节点`
2. 双击创建的CameraBehaviourData资源打开模板编辑器
3. 添加和配置各种行为节点模板
4. 设置行为ID和参数供代码调用

## 📋 示例场景

项目包含完整的演示场景和测试脚本：

- **Demo/root.unity**: 基础功能演示场景
- **TestDemo.cs**: 包含各种功能的测试代码
- **CameraRoot.prefab**: 预配置的相机系统预制体

### TestDemo 功能测试
```csharp
// 按键0: 测试瞬切虚拟相机
// 按键1: 测试缓切虚拟相机  
// 按键2: 测试相机行为树事件
```

## ⚙️ 配置文件

系统使用以下配置资源：
- **基础配置**:CameraTreeEditor.EditorPah 需要自行根据目录结构配置到CameraTree的路径
- **CameraBehaviourData.asset**: 相机行为模板配置
- **CameraTree.asset**: 相机行为树配置
- **CinemachineBlenderSettings.asset**: Cinemachine混合设置

## 📖 API 文档

### CMCameraManager 主要接口

#### 相机状态控制
```csharp
// 切换到自动状态
public void ToAutoState()

// 开始追背动作
public void StartChaseBack(int id, float waitTime = -1, float recentTime = -1)

// 开始瞄准旋转
public void StartRotateToAim(RotateToAimEvent rotateToAimEvent, Transform target)

// 视距缓动控制
public void StartZoomFromToEvent(int id)
```

#### 虚拟相机管理
```csharp
// 瞬切相机控制
public void OpenCutVCamera(Vector3 pos, Quaternion qua, LayerMask? layer = null, float fov = DefaultFov)
public void CloseCutVCamera()

// 缓切相机控制  
public void OpenBlendVCamera(Vector3 pos, Quaternion qua, LayerMask? layer = null, float fov = DefaultFov)
public void CloseBlendVCamera()
```

#### 震动和特效
```csharp
// 播放震动效果
public void PlayShake(float duration, float shakeSensitive)
public void PlayShake(float duration, float shakeSensitive, ImpulseData data)

// 停止震动
public void StopShake()
```

### 状态机接口
```csharp
// 获取当前状态
public FSMState GetFSMState()

// 检查状态类型
public bool IsCurrentFSMState<T>()

// 输入状态机事件
public void InputFSMEvent(FSMEvent e)
```

## 🔧 自定义扩展

### 创建自定义相机状态
```csharp
// 继承CameraFSMState基类
public class CustomCameraState : CameraFSMState
{
    public override FSMState Enter(FSMEvent fsmEvent, FSMState lastState)
    {
        // 进入状态时的逻辑
        return this;
    }

    public override FSMState Tick(float deltaTime)
    {
        // 状态更新逻辑
        return this;
    }

    public override FSMState OnInputEvent(FSMEvent inputEvent)
    {
        // 处理输入事件
        return this;
    }

    public override void Exit(FSMEvent fsmEvent, FSMState nextState)
    {
        // 退出状态时的清理逻辑
    }
}
```

### 创建自定义相机事件
```csharp
// 继承CameraEvent基类
public class CustomCameraEvent : CameraEvent  
{
    public float customParameter;

    public override void Reset()
    {
        base.Reset();
        customParameter = 0f;
    }
}
```

### 创建自定义相机节点
```csharp
// 继承CameraAction基类用于行为树节点
public class CustomCameraNode : CameraAction
{
    [Header("自定义参数")]
    public float customValue = 1.0f;

    public override bool DoAction()
    {
        // 实现自定义相机行为
        Debug.Log($"执行自定义相机动作: {customValue}");
        return base.DoAction();
    }
}
```

## 🎯 设计原理

### 状态机设计
- **状态分离**: 每种相机行为独立成状态，便于维护和扩展
- **事件驱动**: 通过事件触发状态转换，降低耦合度  
- **可复用性**: 状态和事件可在不同场景下复用
- **可扩展性**: 支持自定义状态和事件的轻松接入

### 行为树设计  
- **节点化**: 复杂相机动作分解为简单节点的组合
- **可视化**: 直观的节点编辑器提高开发效率
- **参数化**: 每个节点支持丰富的参数配置
- **复用性**: 行为树可作为资源在多处复用

### 架构优势
- **模块化设计**: 各系统职责清晰，便于团队协作
- **性能优化**: 对象池和状态机减少GC和计算开销  
- **易于调试**: 状态可视化和日志系统便于问题定位
- **平台兼容**: 支持PC和移动平台的输入适配

## 🤝 参与贡献

我们欢迎社区贡献！请遵循以下步骤：

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)  
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

### 贡献指南
- 请确保代码符合项目的编码规范
- 为新功能添加适当的注释和文档
- 测试您的更改以确保不会破坏现有功能
- 更新相关的文档和示例

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [Unity Technologies](https://unity.com/) - Unity 游戏引擎
- [Cinemachine](https://unity.com/packages/essentials/cinemachine) - 专业相机系统
- [GraphView](https://docs.unity3d.com/Manual/UIE-graph-view.html) - Unity 图形视图系统

## 📞 联系方式

如有问题或建议，请通过以下方式联系我们：

- 提交 [Issue](https://github.com/yunzeforbetter/CameraFSM/issues)

---

⭐ 如果这个项目对您有帮助，请给我们一个星标！


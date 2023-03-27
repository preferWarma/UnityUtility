# 说明文档

## 1.数据生成

该dialogue system提供了数据生成的快捷脚本DialogueDataGenerate.cs

***【注】使用该脚本需要使用第三方库ExcelDataReader和ExcelDataReader.setdata***

该脚本的源码如下：

![image-20230312225039290](README.assets/image-20230312225039290.png)

其中assetPath为数据的生成位置, filePath为要读取的Excel表数据

在确定你的生成路径和读取路径后，你可以在Unity中这样使用该生成方法：点击Tool下的“Generate allDialougueData”选项即可

![image-20230312225234712](README.assets/image-20230312225234712.png)

对话数据会以ScriptableObject的形式保存

![image-20230312225359085](README.assets/image-20230312225359085.png)

## 2.对话系统使用

对话系统的主要组成如下:

![image-20230312225510934](README.assets/image-20230312225510934.png)

分别解释每一个脚本的作用：

### BaseDialogueController.cs

- 作为DialogueController的基类，为子类提供存放对话数据的属性接口和通过自身对话数据开启对话的方法接口

### DialogueController.cs

- 【使用方法】：将需要触发对话的GameObject挂载此脚本，设置好该对象对应的对话数据和触发方法即可，

![image-20230312230341340](README.assets/image-20230312230341340.png)

上述例子中我们为GameObject挂载了该脚本，设置了该对象的对话数据，在介绍下面的脚本内容的时候我们会讲自定义触发条件

- 继承于BaseDialogueController，为挂载对象提供任意的触发对话的方法(需自己编写自定义触发条件)，下面是示例

![image-20230312230039908](README.assets/image-20230312230039908.png)

该例中，我们在按下鼠标右键的时候即可开启对话

**【注意！！！】**对于触发条件不同的物体，你有两种办法：

- 新建多个不同的脚本分别挂载，只要它是继承于BaseDialogueController即可，每个脚本单独设置它的触发方法（简单快捷，但是会导致脚本数增加）
- 统一使用相同的DialogueController脚本，在此脚本中通过Tag或者name来区分不同的物体，使其单独触发，下面给出示例（详细注释，不多赘述）

![image-20230312231439671](README.assets/image-20230312231439671.png)

### DialogueData.cs

- 该脚本表示对话数据，继承于ScriptableObject，第一步中生成的数据类型即为此类，类里包含该对话数据的对话列表

![image-20230312231934018](README.assets/image-20230312231934018.png)

### DialogueManager.cs

- 该系统的管理者，该对话系统的核心脚本，采用单例模式，控制对话的显示，隐藏，更新
- 提供各种组件的接口，详细注释，在此不多赘述

![image-20230312233103745](README.assets/image-20230312233103745.png)

篇幅有限，每个方法的具体实现不予说明，有兴趣可以查看源码，含有详细注释，下面给出测试用的挂载信息

【注】对话信息这一栏无需填写，DialogueController会自动为它赋值的

![image-20230312234335552](README.assets/image-20230312234335552.png)

### PieceData.cs

- 单条对话数据，成员变量和Excel表的每一列一一对应

![image-20230327234642115](README.assets/image-20230327234642115.png)
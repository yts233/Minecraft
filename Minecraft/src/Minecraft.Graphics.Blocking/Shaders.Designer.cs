﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Minecraft.Graphics.Blocking {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Shaders {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Shaders() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Minecraft.Graphics.Blocking.Shaders", typeof(Shaders).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 #version 330 core
        ///in vec4 color;
        ///in vec2 texCoord;
        ///out vec4 FragColor;
        ///uniform sampler2D texture1;
        ///
        ///void main()
        ///{
        ///    FragColor = texture(texture1, texCoord) * color;
        ///}  的本地化字符串。
        /// </summary>
        internal static string BlockFragmentShaderSource {
            get {
                return ResourceManager.GetString("BlockFragmentShaderSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 #version 330 core
        ///layout (location = 0) in vec3 aPos;
        ///layout (location = 1) in vec2 aTex;
        ///layout (location = 2) in vec3 aNor;
        ///out vec2 texCoord;
        ///out vec4 color;
        ///
        ///uniform mat4 model;
        ///uniform mat4 view;
        ///uniform mat4 projection;
        ///
        ///void main()
        ///{
        ///    gl_Position = projection * view * model * vec4(aPos, 1.0F);
        ///    //gl_Position = vec4(aPos, 1.0F);
        ///    texCoord = vec2(aTex.x, 1.0F - aTex.y);
        ///    const float amb = 0.7F;
        ///    const float dlig = 0.3F;
        ///    const vec3 lightDir = normalize(vec3(1.0F,3.0F,2.0F));
        ///    float [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string BlockVertexShaderSource {
            get {
                return ResourceManager.GetString("BlockVertexShaderSource", resourceCulture);
            }
        }
    }
}
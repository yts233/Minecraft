﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Minecraft.Graphics.Renderers.Environments {
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
    internal class EnvironmentShaders {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EnvironmentShaders() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Minecraft.Graphics.Renderers.Environments.EnvironmentShaders", typeof(EnvironmentShaders).Assembly);
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
        ///in vec3 deltaPos;
        ///in vec3 objectColor;
        ///in vec3 normal;
        ///out vec4 FragColor;
        ///
        ///void main() {
        ///    const vec3 up = vec3(0F, 1F, 0F);
        ///    vec3 lightColor = vec3(1F);
        ///    vec3 ambient = 0.8F * lightColor;
        ///    float diffA = max(dot(normal, normalize(vec3(1.0F,5.0F,1.0F))), 0.0);
        ///    float diffB = max(dot(normal, normalize(vec3(-2.0F,1.0F,-1.0F))), 0.0);
        ///    vec3 diffuseA = diffA * lightColor * .7F;
        ///    vec3 diffuseB = diffB * lightColor * .1F;
        ///    float skySpec = max(min(512 / length [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string CloudFragmentShaderSource {
            get {
                return ResourceManager.GetString("CloudFragmentShaderSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 #version 330 core
        ///layout (location = 0) in vec3 aPos;
        ///layout (location = 1) in vec3 aNormal;
        ///out vec3 objectColor;
        ///out vec3 normal;
        ///out vec3 deltaPos;
        ///uniform vec3 color;
        ///uniform vec3 centerPosition;
        ///uniform vec2 position;
        ///uniform float offset;
        ///uniform mat4 view;
        ///uniform mat4 projection;
        ///void main() {
        ///    vec3 worldPosition = vec3(aPos.x * 12 + position.x * 12, aPos.y * 4 + 128, aPos.z * 12 + position.y * 12 - offset);
        ///    gl_Position = projection * view * vec4(worldPosition, 1.0F);
        ///    objec [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string CloudVertexShaderSource {
            get {
                return ResourceManager.GetString("CloudVertexShaderSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 #version 330 core
        ///in vec2 texCoord;
        ///out vec4 FragColor;
        ///
        ///uniform sampler2D texture1;
        ///
        ///void main() {
        ///    FragColor = texture(texture1,texCoord);
        ///}
        /// 的本地化字符串。
        /// </summary>
        internal static string SkyboxFragmentShaderSource {
            get {
                return ResourceManager.GetString("SkyboxFragmentShaderSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 #version 330 core
        ///layout (location = 0) in vec3 aPos;
        ///layout (location =1)in vec2 aTex;
        ///
        ///out vec2 texCoord;
        ///
        ///uniform mat4 view;
        ///uniform mat4 model;
        ///uniform mat4 projection;
        ///
        ///void main() {
        ///    gl_Position = projection * view * model * vec4(aPos, 1.0F);
        ///    texCoord = aTex;
        ///}
        /// 的本地化字符串。
        /// </summary>
        internal static string SkyboxVertexShaderSource {
            get {
                return ResourceManager.GetString("SkyboxVertexShaderSource", resourceCulture);
            }
        }
    }
}
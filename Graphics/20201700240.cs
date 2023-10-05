using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;

using System.IO;
using System.Diagnostics;

namespace Graphics
{
    class Renderer
    {
        Shader sh;

        int S;
        Texture tex;

        uint cuboidBufferID;
        uint cuboid2BufferID;
        uint cuboid3BufferID;
        uint pyramidBufferID;
        uint groundBufferID;
        uint sunBufferID;
        uint xyzAxesBufferID;

        //3D Drawing
        mat4 ModelMatrix_sun;
        mat4 ModelMatrix_pyramid;
        mat4 ModelMatrix_ground;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
        
        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        int EDGES_NUMBER = 9;
        vec3 sunCenter;
        vec3 scaling_ground = new vec3(1, 1, 1);
        //vec3 scaling_pyramid = new vec3(1, 1, 1);

        const float rotationSpeed = 15f;
        float rotationAngle = 0;
       
        public float translationX=0, 
                     translationY=0, 
                     translationZ=0;

        Stopwatch timer = Stopwatch.StartNew();

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath 
                + "\\Shaders\\20201700240.fragmentshader");

            tex = new Texture(projectPath + "\\Textures\\Sand.jpg", 1);
           
            Gl.glClearColor(0, 0, 0.4f, 1);
            
            float[] cuboidVertices = { 
		            //base
		            0.0f, 0.0f, 0.0f,      0.0f, 1.0f, 0.0f, //green
	                30.0f, 0.0f, 0.0f,     0.0f, 1.0f, 0.0f,
		            30.0f, 30.0f, 0.0f,    0.0f, 1.0f, 0.0f,
                    0.0f, 30.0f, 0.0f,     0.0f, 1.0f, 0.0f,

                   //top
		            0.0f, 0.0f, 5.0f,      0.0f, 0.0f, 1.0f, //blue
                    30.0f, 0.0f, 5.0f,     0.0f, 0.0f, 1.0f,
                    30.0f, 30.0f, 5.0f,    0.0f, 0.0f, 1.0f,
                    0.0f, 30.0f, 5.0f,     0.0f, 0.0f, 1.0f,
                
                    //back
		            0.0f, 0.0f, 0.0f,      0.0f, 1.0f, 1.0f, //light blue
                    0.0f, 0.0f, 5.0f,      0.0f, 1.0f, 1.0f,
                    0.0f, 30.0f, 5.0f,     0.0f, 1.0f, 1.0f,
                    0.0f, 30.0f, 0.0f,     0.0f, 1.0f, 1.0f,
                 
                    //left
		            30.0f, 0.0f, 0.0f,     1.0f, 1.0f, 0.0f, //yellow
                    30.0f, 0.0f, 5.0f,     1.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 5.0f,      1.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f,      1.0f, 1.0f, 0.0f,
                
                    //right
		            30.0f, 30.0f, 0.0f,    1.0f, 0.0f, 0.0f, //red
                    30.0f, 30.0f, 5.0f,    1.0f, 0.0f, 0.0f,
                    0.0f, 30.0f, 5.0f,     1.0f, 0.0f, 0.0f,
                    0.0f, 30.0f, 0.0f,     1.0f, 0.0f, 0.0f,
               
                    //front
		            30.0f, 30.0f, 0.0f,    1.0f, 0.0f, 1.0f, //pink
                    30.0f, 30.0f, 5.0f,    1.0f, 0.0f, 1.0f,
                    30.0f, 0.0f, 5.0f,     1.0f, 0.0f, 1.0f,
                    30.0f, 0.0f, 0.0f,     1.0f, 0.0f, 1.0f,
            };
           
            float[] cuboid2Vertices = { 
		            //base
		            5.0f, 5.0f, 5.0f,      0.0f, 1.0f, 0.0f, //green
	                25.0f, 5.0f, 5.0f,     0.0f, 1.0f, 0.0f,
                    25.0f, 25.0f, 5.0f,    0.0f, 1.0f, 0.0f,
                    5.0f, 25.0f, 5.0f,     0.0f, 1.0f, 0.0f,

                   //top
		            5.0f, 5.0f, 10.0f,     0.0f, 0.0f, 1.0f, //blue
                    25.0f, 5.0f, 10.0f,    0.0f, 0.0f, 1.0f,
                    25.0f, 25.0f, 10.0f,   0.0f, 0.0f, 1.0f,
                    5.0f, 25.0f, 10.0f,    0.0f, 0.0f, 1.0f,

                
                   //back
		            5.0f, 5.0f, 5.0f,      0.0f, 1.0f, 1.0f, //light blue
                    5.0f, 5.0f, 10.0f,     0.0f, 1.0f, 1.0f,
                    5.0f, 25.0f, 10.0f,    0.0f, 1.0f, 1.0f,
                    5.0f, 25.0f, 5.0f,     0.0f, 1.0f, 1.0f,
                
                    //left
		            25.0f, 5.0f, 5.0f,     1.0f, 1.0f, 0.0f, //yellow
                    25.0f, 5.0f, 10.0f,    1.0f, 1.0f, 0.0f,
                    5.0f, 5.0f, 10.0f,     1.0f, 1.0f, 0.0f,
                    5.0f, 5.0f, 5.0f,      1.0f, 1.0f, 0.0f,
                
                    //right
		            25.0f, 25.0f, 5.0f,    1.0f, 0.0f, 0.0f, //red
                    25.0f, 25.0f, 10.0f,   1.0f, 0.0f, 0.0f,
                    5.0f, 25.0f, 10.0f,    1.0f, 0.0f, 0.0f,
                    5.0f, 25.0f, 5.0f,     1.0f, 0.0f, 0.0f,
               
                    //front
		            25.0f, 25.0f, 5.0f,    1.0f, 0.0f, 1.0f, //pink
                    25.0f, 25.0f, 10.0f,   1.0f, 0.0f, 1.0f,
                    25.0f, 5.0f, 10.0f,    1.0f, 0.0f, 1.0f,
                    25.0f, 5.0f, 5.0f,     1.0f, 0.0f, 1.0f,
            };

            float[] cuboid3Vertices = { 
		            //base
		            10.0f, 10.0f, 10.0f,    0.0f, 1.0f, 0.0f, //green
	                20.0f, 10.0f, 10.0f,    0.0f, 1.0f, 0.0f,
                    20.0f, 20.0f, 10.0f,    0.0f, 1.0f, 0.0f,
                    10.0f, 20.0f, 10.0f,    0.0f, 1.0f, 0.0f,

                   //top
		            10.0f, 10.0f, 15.0f,    0.0f, 0.0f, 1.0f, //blue
                    20.0f, 10.0f, 15.0f,    0.0f, 0.0f, 1.0f,
                    20.0f, 20.0f, 15.0f,    0.0f, 0.0f, 1.0f,
                    10.0f, 20.0f, 15.0f,    0.0f, 0.0f, 1.0f,

                
                   //back
		            10.0f, 10.0f, 10.0f,    0.0f, 1.0f, 1.0f, //light blue
                    10.0f, 10.0f, 15.0f,    0.0f, 1.0f, 1.0f,
                    10.0f, 20.0f, 15.0f,    0.0f, 1.0f, 1.0f,
                    10.0f, 20.0f, 10.0f,    0.0f, 1.0f, 1.0f,
                
                    //left
		            20.0f, 10.0f, 10.0f,    1.0f, 1.0f, 0.0f, //yellow
                    20.0f, 10.0f, 15.0f,    1.0f, 1.0f, 0.0f,
                    10.0f, 10.0f, 15.0f,    1.0f, 1.0f, 0.0f,
                    10.0f, 10.0f, 10.0f,    1.0f, 1.0f, 0.0f,
                
                    //right
		            20.0f, 20.0f, 10.0f,    1.0f, 0.0f, 0.0f, //red
                    20.0f, 20.0f, 15.0f,    1.0f, 0.0f, 0.0f,
                    10.0f, 20.0f, 15.0f,    1.0f, 0.0f, 0.0f,
                    10.0f, 20.0f, 10.0f,    1.0f, 0.0f, 0.0f,
              
                    //front
		            20.0f, 20.0f, 10.0f,    1.0f, 0.0f, 1.0f, //pink
                    20.0f, 20.0f, 15.0f,    1.0f, 0.0f, 1.0f,
                    20.0f, 10.0f, 15.0f,    1.0f, 0.0f, 1.0f,
                    20.0f, 10.0f, 10.0f,    1.0f, 0.0f, 1.0f,
            };

            float[] pyramidVertices = { 

                    //back
		            12.5f, 17.5f, 15.0f,    0.0f, 1.0f, 1.0f, //light blue
                    12.5f, 12.5f, 15.0f,    0.0f, 1.0f, 1.0f,
                    17.5f, 17.5f, 17.5f,    0.0f, 1.0f, 1.0f,
                
                    //left
                    12.5f, 12.5f, 15.0f,    1.0f, 1.0f, 0.0f, //yellow
                    17.5f, 12.5f, 15.0f,    1.0f, 1.0f, 0.0f,
                    17.5f, 17.5f, 17.5f,    1.0f, 1.0f, 0.0f, 

                    //right
		            12.5f, 17.5f, 15.0f,    1.0f, 0.0f, 0.0f, //red
                    17.5f, 17.5f, 15.0f,    1.0f, 0.0f, 0.0f,
                    17.5f, 17.5f, 17.5f,    1.0f, 0.0f, 0.0f, 
               
                    //front
		            17.5f, 17.5f, 15.0f,    1.0f, 0.0f, 1.0f, //pink
                    17.5f, 12.5f, 15.0f,    1.0f, 0.0f, 1.0f,
                    17.5f, 17.5f, 17.5f,    1.0f, 0.0f, 1.0f,

            }; 

            float[] groundVertices = { 
		             
                    //base
		            0.0f, 0.0f, -0.5f,      0.0f, 1.0f, 0.0f,   0,0,//green
	                60.0f, 0.0f, -0.5f,     0.0f, 1.0f, 0.0f,   0,1,
                    60.0f, 60.0f, -0.5f,    0.0f, 1.0f, 0.0f,   1,1,
                    0.0f, 60.0f, -0.5f,     0.0f, 1.0f, 0.0f,   1,0,

                    /*//base
		            0.0f, 0.0f, -0.5f,      0.0f, 1.0f, 0.0f,   0,0,//green
	                60.0f, 0.0f, -0.5f,     0.0f, 1.0f, 0.0f,   0,1,
                    60.0f, 60.0f, -0.5f,    0.0f, 1.0f, 0.0f,   1,1,


                    0.0f, 0.0f, -0.5f,      0.0f, 1.0f, 0.0f,   0,0,//green
                    0.0f, 60.0f, -0.5f,     0.0f, 1.0f, 0.0f,   0,1,
                    60.0f, 60.0f, -0.5f,    0.0f, 1.0f, 0.0f,   1,1,*/

            };
            
            float[] xyzAxesVertices = {
		            //x
		            0.0f, 0.0f, 0.0f,       1.0f, 0.0f, 0.0f, //red
		            80.0f, 0.0f, 0.0f,      1.0f, 0.0f, 0.0f, 
		            //y
	                0.0f, 0.0f, 0.0f,       0.0f, 1.0f, 0.0f, //green
		            0.0f, 80.0f, 0.0f,      0.0f, 1.0f, 0.0f, 
		            //z
	                0.0f, 0.0f, 0.0f,       0.0f, 0.0f, 1.0f, //blue
		            0.0f, 0.0f, 80.0f,      0.0f, 0.0f, 1.0f,  
                };

            float[] sunVertices = DrawCircle(7.5f, 7.5f, 32.0f, EDGES_NUMBER, 1, 1, 0);
            sunCenter = new vec3(7.5f, 7.5f, 32.0f);

            cuboidBufferID = GPU.GenerateBuffer(cuboidVertices);
            cuboid2BufferID = GPU.GenerateBuffer(cuboid2Vertices);
            cuboid3BufferID = GPU.GenerateBuffer(cuboid3Vertices);
            pyramidBufferID = GPU.GenerateBuffer(pyramidVertices);
            groundBufferID = GPU.GenerateBuffer(groundVertices);
            sunBufferID = GPU.GenerateBuffer(sunVertices);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(50, 50, 50), // Camera is at (0,5,5), in World Space
                        new vec3(0, 0, 0), // and looks at the origin
                        new vec3(0, 0, 1)  // Head is up (set to 0,-1,0 to look upside-down)
                );
            // Model Matrix Initialization
            ModelMatrix_sun = new mat4(1);
            ModelMatrix_pyramid = new mat4(1);
            ModelMatrix_ground = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);
            
            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");
            S = Gl.glGetUniformLocation(sh.ID, "c");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            #region XYZ axis
                Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

                Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array()); // Identity

                Gl.glEnableVertexAttribArray(0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                Gl.glUniform1i(S, 0);
                Gl.glDrawArrays(Gl.GL_LINES, 0, 6);

                Gl.glDisableVertexAttribArray(0);
                Gl.glDisableVertexAttribArray(1);

            #endregion

            #region Animated Ground
            
                GPU.BindBuffer(groundBufferID);
                //Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, groundBufferID);

                Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_ground.to_array());

                Gl.glEnableVertexAttribArray(0);
                //Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
                Gl.glEnableVertexAttribArray(2);
                Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

                tex.Bind();
                Gl.glUniform1i(S, 1);
                Gl.glDrawArrays(Gl.GL_POLYGON, 0, 4);
                /*Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);

                Gl.glUniform1i(S, 0);
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 3, 3);*/

                Gl.glDisableVertexAttribArray(0);
                Gl.glDisableVertexAttribArray(1);
                Gl.glDisableVertexAttribArray(2);
            #endregion

            #region Animated Cuboid
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cuboidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_pyramid.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniform1i(S, 0);
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6 * 4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Animated Cuboid2
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cuboid2BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_pyramid.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniform1i(S, 0);
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6 * 4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Animated Cuboid3
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cuboid3BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_pyramid.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniform1i(S, 0);
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6 * 4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Animated Pyramid
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, pyramidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_pyramid.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniform1i(S, 0);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 4 * 3);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Animated Sun
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, sunBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix_sun.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniform1i(S, 0);
            Gl.glDrawArrays(Gl.GL_TRIANGLE_FAN, 0, EDGES_NUMBER);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

        }

        public float[] DrawCircle(float cntrX, float cntrY, float cntrZ, float r, float Red, float Green, float Blue)
        {
            List<float> vrtx = new List<float>();

            float edge = (float)(2 * Math.PI) / EDGES_NUMBER;

            float theta = 0.0f;
            while (theta < (2 * Math.PI))
            {
                float x = cntrX + (float)(r * Math.Cos(theta));
                float z = cntrZ + (float)(r * Math.Sin(theta));
                vrtx.AddRange(new float[] { x, cntrY, z, Red, Green, Blue });
                theta += edge;
            }

            return vrtx.ToArray();
        }
        
        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds/1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            //sun rotatation
            List<mat4> transform_sun = new List<mat4>();
            transform_sun.Add(glm.translate(new mat4(1), -1 * sunCenter));
            transform_sun.Add(glm.rotate(rotationAngle, new vec3(0, 1, 0)));
            transform_sun.Add(glm.translate(new mat4(1), sunCenter));
            ModelMatrix_sun = MathHelper.MultiplyMatrices(transform_sun);

            //pyramid translation
            List<mat4> transform_pyramid = new List<mat4>();
            transform_pyramid.Add(glm.translate(new mat4(1), new vec3(0, 0, 0)));
            //transform_pyramid.Add(glm.scale(new mat4(1), scaling_pyramid));
            transform_pyramid.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));
            ModelMatrix_pyramid = MathHelper.MultiplyMatrices(transform_pyramid);

            //ground scaling
            List<mat4> transform_ground = new List<mat4>();
            transform_ground.Add(glm.scale(new mat4(1), scaling_ground));
            ModelMatrix_ground = MathHelper.MultiplyMatrices(transform_ground);

            timer.Reset();
            timer.Start();
        }

        public void keyPress(char k)
        {

            float speed = 5;
            float scale = 1.5f;

            if (k == 'd')
            {
                translationX += speed;
                //scaling_pyramid.x *= scale;
                
            }
            if (k == 'a')
            {
                translationX -= speed;
                //scaling_pyramid.x /= scale;
                
            }
            if (k == 'w')
            {
                translationY += speed;
               
                //scaling_pyramid.y *= scale;
            }
            if (k == 's')
            {
                translationY -= speed;
                
                //scaling_pyramid.y /= scale;
            }
            if (k == 'z')
            {
                translationZ += speed;
                //scaling_pyramid.x *= scale;
                //scaling_pyramid.y *= scale;
            }
            if (k == 'c')
            {
                translationZ -= speed;
                //scaling_pyramid.x /= scale;
                //scaling_pyramid.y /= scale;
            }


            if (k == '+')
            {
                scaling_ground.x *= scale;
                scaling_ground.y *= scale;
            }

            if (k == '-')
            {
                scaling_ground.x /= scale;
                scaling_ground.y /= scale;
            }
        }
        
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}

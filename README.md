# Frustm Visibility System

## Purpose
The Frustum Visibility System is designed to optimize scene performance in Unity by dynamically adjusting the visibility and shadow casting properties of objects based on their presence within the camera's view frustum. This system helps in reducing the rendering load by only processing objects that are within the player's field of view. For a deeper understanding of camera frustums in Unity, refer to the Unity Manual on Camera's View Frustum.
- https://docs.unity3d.com/Manual/UnderstandingFrustum.html

## How to Use
- **Drag and Drop:** Drag and Drop: To implement the Frustum Visibility System, simply drag and drop the FrustumVisibilityManager script onto any active camera in your scene. Ensure that objects that need visibility management are equipped with the VisibilitySubject script.
- **Adjust Settings:** Customize settings such as check intervals, visibility types, and visibility offsets in the FrustumVisibilityManager script according to your specific requirements.
- **Runtime Efficiency:** This system automatically updates the visibility state of objects based on player movement and predefined intervals, ensuring optimal performance during runtime.

## Package Overview
This package provides essential scripts (FrustumVisibilityManager and VisibilitySubject) that enable the dynamic management of object visibility and shadow casting based on the camera's view frustum. It is designed for easy integration and minimal setup, making it an ideal solution for performance optimization in various Unity projects.

## Performance Comparison
<img width="321" alt="Screenshot 2024-04-20 at 20 07 29" src="https://github.com/safakgulpinar/FrustumVisibilitySystem/assets/90358778/3a6951fc-172a-4adf-8442-356b34c2889a">
<img width="323" alt="Screenshot 2024-04-20 at 20 11 41" src="https://github.com/safakgulpinar/FrustumVisibilitySystem/assets/90358778/0213042d-ee30-4f39-a783-6ced2c09315f">

![Basic Wobbly TMP Video](https://github.com/safakgulpinar/FrustumVisibilitySystem/assets/90358778/0f0eedfd-0409-44f5-abb2-78274abede33)




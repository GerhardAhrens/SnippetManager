# Snippet Manager

![NET](https://img.shields.io/badge/NET-10-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2026](https://img.shields.io/badge/Visual%20Studio-2026-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2026.1-yellow.svg)

# Projekt

Der Snippet Manager ist eine Anwendung, die es Benutzern ermöglicht, Code-Snippets zu erstellen, zu organisieren und zu verwalten. Es bietet eine benutzerfreundliche Oberfläche, um Snippets schnell zu speichern und wiederzuverwenden.

## Hauptdialog

<img src="MainWindow.png" style="width:650px;"/>

## Snippet Dialog

## XAML Icon Dialog
Übersicht von XAML Icons auf Basis vom Typ ``DrawingImage``. Diese können in der gesamten Anwendung verwendet werden, um konsistente und ansprechende Symbole darzustellen.\
in dem Dialog kann nach Namen gefiltert werden. Bei einem Doppelklick auf das Symbol wird der XAML Source in die Zwischenablage kopiert.
<img src="XamlIconUC.png" style="width:650px;"/>

Ergebnis nach dem Einfügen aus der Zwischenablage.
```XAML
<DrawingImage x:Key="IconChat">
  <DrawingImage.Drawing>
    <DrawingGroup>
      <DrawingGroup.Children>
        <GeometryDrawing Brush="#FFD6D6D6">
          <GeometryDrawing.Geometry>
            <PathGeometry Figures="M11,3L21,3Q24,3,24,6L24,14Q24,17,21,17L17,17L13,21L13,17L11,17Q8,17,8,14L8,6Q8,3,11,3z" />
          </GeometryDrawing.Geometry>
        </GeometryDrawing>
        <GeometryDrawing Brush="#FF4A90E2">
          <GeometryDrawing.Geometry>
            <PathGeometry Figures="M4,7L17,7Q20,7,20,10L20,18Q20,21,17,21L10,21L6,24L6,21L4,21Q1,21,1,18L1,10Q1,7,4,7z" />
          </GeometryDrawing.Geometry>
        </GeometryDrawing>
        <GeometryDrawing Brush="#FFFFFFFF">
          <GeometryDrawing.Geometry>
            <EllipseGeometry RadiusX="1.4" RadiusY="1.4" Center="7,14" />
          </GeometryDrawing.Geometry>
        </GeometryDrawing>
        <GeometryDrawing Brush="#FFFFFFFF">
          <GeometryDrawing.Geometry>
            <EllipseGeometry RadiusX="1.4" RadiusY="1.4" Center="11,14" />
          </GeometryDrawing.Geometry>
        </GeometryDrawing>
        <GeometryDrawing Brush="#FFFFFFFF">
          <GeometryDrawing.Geometry>
            <EllipseGeometry RadiusX="1.4" RadiusY="1.4" Center="15,14" />
          </GeometryDrawing.Geometry>
        </GeometryDrawing>
      </DrawingGroup.Children>
    </DrawingGroup>
  </DrawingImage.Drawing>
</DrawingImage>
```

# Features
- Import von Xaml Icons Dateien vom Typ ``DrawingImage`` und ``Viewbox``.
- Ein Icon aus der Datenbank kann per Doppelklick als XAML Code vom Typ ``DrawingImage`` in die Zwischenablage kopiert werden.
- Ein Auswahl von Icons kann als XAML Code vom Typ ``DrawingImage`` können in ein Resource Dictionary gesammelt und in die Zwischenablage kopiert werden.

# Versionshistorie

![Version](https://img.shields.io/badge/Version-1.0.2026.1-yellow.svg)
- Erste Version

pipeline {
    agent any

    environment {
        SCANNER_HOME = tool('SonarScanner')
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                echo "No build needed"
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat """
                    "%SCANNER_HOME%\\bin\\sonar-scanner.bat" ^
                    -Dsonar.projectKey=PhotographyPortfolio ^
                    -Dsonar.sources=. ^
                    -Dsonar.host.url=http://localhost:9000
                    """
                }
            }
        }
    }
}

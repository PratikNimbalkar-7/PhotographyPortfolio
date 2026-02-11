pipeline {
    agent any

    environment {
        SCANNER_HOME = tool('SonarScanner')
        PROJECT_KEY = "PhotographyPortfolio-${env.BRANCH_NAME}"
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
                    -Dsonar.projectKey=%PROJECT_KEY% ^
                    -Dsonar.projectName=%PROJECT_KEY% ^
                    -Dsonar.sources=. ^
                    -Dsonar.host.url=http://localhost:9000
                    """
                }
            }
        }
    }
}

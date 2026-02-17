pipeline {

    agent any

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test'
            }
        }

    }

    post {
        success {
            echo "Build Successful for ${env.BRANCH_NAME}"
        }

        failure {
            echo "Build Failed for ${env.BRANCH_NAME}"
        }
    }
}
